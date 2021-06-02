using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using Database.Data;

namespace Database.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("[controller]")]
    public class NluController : ControllerBase
    {
        private readonly ILogger<NluController> _logger;
        private readonly QAContext _context;
        private readonly HttpClient client;

        public NluController(QAContext context, ILogger<NluController> logger)
        {
            _context = context;
            _logger = logger;
            client = new HttpClient();
            var nlu_host = Environment.GetEnvironmentVariable("NLU_HOST");
            var nlu_port = Int16.Parse(Environment.GetEnvironmentVariable("NLU_PORT"));
            client.BaseAddress = new UriBuilder("http", nlu_host, nlu_port).Uri;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var response = await client.GetAsync("/");
            return await response.Content.ReadAsStringAsync();
        }

        [HttpGet("version")]
        public async Task<Nlu.Api.Information.Version> Version()
        {
            var response = await client.GetAsync("/version");
            return await response.Content.ReadFromJsonAsync<Nlu.Api.Information.Version>();
        }

        [HttpGet("status")]
        public async Task<Nlu.Api.Information.Status> Status()
        {
            var response = await client.GetAsync("/status");
            return await response.Content.ReadFromJsonAsync<Nlu.Api.Information.Status>();
        }

        [HttpGet("domain")]
        public async Task<Nlu.Api.Domain.Domain> Domain()
        {
            var response = await client.GetAsync("/domain");
            return await response.Content.ReadFromJsonAsync<Nlu.Api.Domain.Domain>();
        }

        [HttpPost("parse")]
        public async Task<Nlu.Api.Parse.ParseResult> Parse([FromBody] string text)
        {
            var data = new Dictionary<string, string> { { "text", text } };
            var content = JsonContent.Create(data);
            var response = await client.PostAsync("/model/parse", content);
            return await response.Content.ReadFromJsonAsync<Nlu.Api.Parse.ParseResult>();
        }

        [HttpPost("test")]
        public async Task<Nlu.Api.Test.TestResult> Test(
        //public async Task<object> Test(
            [FromQuery] string model = null,
            [FromQuery] string callback_url = null,
            [FromQuery] int? cross_validation_folds = null,
            [FromBody] object common_examples = null
        )
        {
            string uri = new Uri(client.BaseAddress, "/model/test/intents").ToString();
            if (model != null)
            {
                uri = QueryHelpers.AddQueryString(uri, "model", model);
            }
            if (callback_url != null)
            {
                uri = QueryHelpers.AddQueryString(uri, "callback_url", callback_url);
            }
            if (cross_validation_folds != null)
            {
                uri = QueryHelpers.AddQueryString(uri, "cross_validation_folds", cross_validation_folds.ToString());
            }

            var content = new Dictionary<string, Dictionary<string, object>>();
            content.Add("rasa_nlu_data", new Dictionary<string, object>());
            content["rasa_nlu_data"].Add("common_examples", common_examples);
            var body = JsonContent.Create(content);

            var response = await client.PostAsync(uri, body);
            return await response.Content.ReadFromJsonAsync<Nlu.Api.Test.TestResult>();
        }

        [HttpPatch("patch")]
        public async Task<byte[]> Patch(
            [FromQuery] bool? save_to_default_model_directory = null,
            [FromQuery] bool? force_training = null,
            [FromQuery] int? augmentation = null,
            [FromQuery] int? num_threads = null,
            [FromQuery] string callback_url = null
        )
        {
            string uri = new Uri(client.BaseAddress, "/model/train").ToString();
            if (save_to_default_model_directory != null)
            {
                uri = QueryHelpers.AddQueryString(uri, "save_to_default_model_directory", save_to_default_model_directory.ToString());
            }
            if (force_training != null)
            {
                uri = QueryHelpers.AddQueryString(uri, "force_training", force_training.ToString());
            }
            if (augmentation != null)
            {
                uri = QueryHelpers.AddQueryString(uri, "augmentation", augmentation.ToString());
            }
            if (num_threads != null)
            {
                uri = QueryHelpers.AddQueryString(uri, "num_threads", num_threads.ToString());
            }
            if (callback_url != null)
            {
                uri = QueryHelpers.AddQueryString(uri, "callback_url", callback_url);
            }

            Nlu.Config.Config Config;
            using (var reader = new StreamReader(Environment.GetEnvironmentVariable("NLU_CONFIG_FILE")))
            {
                Config = new DeserializerBuilder().Build().Deserialize<Nlu.Config.Config>(reader.ReadToEnd());
            }
            Config.intents = _context.Subtopics.Select(st => $"t{st.TopicId}_st{st.Id}").ToList();
            Config.nlu = _context
                .Answers
                .ToList()
                .Join(
                    _context.Questions.Include(q => q.Subtopic),
                    a => a.Id,
                    q => q.AnswerId,
                    (a, q) => new
                    {
                        Intent =  $"t{q.Subtopic.TopicId}_st{q.SubtopicId}/a{q.AnswerId}",
                        Example = q.Value
                    }
                )
                .GroupBy(i => i.Intent)
                .Select(i => new Nlu.Config.Config.Intent
                {
                    intent = i.Key,
                    examples = String.Join("\n", i.Select(q => $"- {q.Example}"))
                });

            var responses = _context
                .Answers
                .ToList()
                .Join(
                    _context.Questions.Include(q => q.Subtopic),
                    a => a.Id,
                    q => q.AnswerId,
                    (a, q) => new
                    {
                        Action = $"utter_t{q.Subtopic.TopicId}_st{q.SubtopicId}/a{a.Id}",
                        Answer = a.Value
                    }
                )
                .GroupBy(r => r.Action)
                .ToDictionary(
                    r => r.Key,
                    r => r.Select(i => new Nlu.Config.Config.Response { text = i.Answer })
                );
            Config.responses = Config
                .responses
                .Concat(responses)
                .GroupBy(r => r.Key, r => r.Value)
                .ToDictionary(r => r.Key, r => r.First());

            var rules = _context
                .Subtopics
                .Select(st => new Nlu.Config.Config.Rule
                {
                    rule =  $"t{st.TopicId}_st{st.Id}",
                    steps = new List<object> {
                            new { intent =  $"t{st.TopicId}_st{st.Id}" },
                            new { action =  $"utter_t{st.TopicId}_st{st.Id}" }
                    }
                });
            Config.rules = Config.rules.Concat(rules);

            var Serializer = new SerializerBuilder().Build();
            var yaml = Serializer.Serialize(Config);
            var body = new StringContent(yaml, System.Text.Encoding.UTF8, "application/x-yaml");
            body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-yaml");
            var response = await client.PostAsync(uri, body);
            var model = response.Content.ReadAsByteArrayAsync();
            var model_file = response.Headers.GetValues("filename").First();
            response = await client.PutAsync("/model", JsonContent.Create(new Dictionary<string, string> {
                { "model_file", "models/" + model_file }
            }));
            return await model;
        }
    }
}
