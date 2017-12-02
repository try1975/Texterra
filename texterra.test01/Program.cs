using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace texterra.test01
{
    internal static class Program
    {
        private static void Main()
        {

            const string responceText = @"[
                {
                    ""text"": ""Самыми высокооплачиваемыми профессиями в России являются капитан морского и командир воздушного судов с зарплатами 500 тысяч и 320 тысяч рублей в месяц соответственно. Об этом во вторник, 26 сентября, сообщают «Известия» со ссылкой на рейтинг самых прибыльных профессий, составленный Минтрудом на основе данных о вакансиях в первом полугодии 2017 года."",
                    ""annotations"": {
                        ""named-entity"": [
                            {
                                ""start"": 41,
                                ""end"": 47,
                                ""value"": {
                                    ""type"": ""bbn"",
                                    ""tag"": ""GPE_COUNTRY""
                                }
                            },
                            {
                                ""start"": 211,
                                ""end"": 219,
                                ""value"": {
                                    ""type"": ""bbn"",
                                    ""tag"": ""ORGANIZATION_OTHER""
                                }
                            }
                        ]
                    }
                }
            ]";
            var dto = JsonConvert.DeserializeObject<List<NamedEntityDto>>(responceText);
            foreach(var namedEntityDto in dto)
            {
                foreach(var ne in namedEntityDto.Annotations.NamedEntity)
                {
                    Console.WriteLine(ne.Start);
                }
            }

            var res = GetTexterraResult("Самыми высокооплачиваемыми профессиями в России являются капитан морского и командир воздушного судов с зарплатами 500 тысяч и 320 тысяч рублей в месяц соответственно. Об этом во вторник, 26 сентября, сообщают «Известия» со ссылкой на рейтинг самых прибыльных профессий, составленный Минтрудом на основе данных о вакансиях в первом полугодии 2017 года.").Result;
        }

        private static async Task<List<NamedEntityDto>> GetTexterraResult(string text)
        {
            using (var httpClient = new HttpClient())
            {
                //httpClient.BaseAddress = new Uri("");
                var baseAddress = "http://api.ispras.ru/texterra/v3.1/nlp?targetType=named-entity&apikey=2d3fde85b374c93dae613c3ff3502d499397212f";
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var texterraText = new[]{new TexterraText { Text = text }};
                Console.WriteLine(JsonConvert.SerializeObject(texterraText));
                using (var response = await httpClient.PostAsJsonAsync(baseAddress, texterraText))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        //Logger.Log.Error($"{nameof(PostFetcheeTask)}: {nameof(response.StatusCode)}={response.StatusCode} {response.ReasonPhrase}={response.ReasonPhrase}");
                        return null;
                    }

                    var result = await response.Content.ReadAsAsync<List<NamedEntityDto>>();
                    return result;
                }
            }
        }
    }

    
    public class TexterraText
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }


    public class NamedEntityDto
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("annotations")]
        public Annotations Annotations { get; set; }
    }

    public class Annotations
    {
        [JsonProperty("named-entity")]
        public List<NamedEntity> NamedEntity { get; set; }
    }

    public class NamedEntities
    {
        public List<NamedEntity> NamedEntity { get; set; }
    }

    public class NamedEntity
    {
        [JsonProperty("start")]
        public int Start { get; set; }
        [JsonProperty("end")]
        public int End { get; set; }
        [JsonProperty("value")]
        public Value Value { get; set; }
    }

    public class Value
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("tag")]
        public string Tag { get; set; }
    }

}
