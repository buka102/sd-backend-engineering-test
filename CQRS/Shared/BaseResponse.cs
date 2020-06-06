using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tictactoe_service.CQRS.Shared
{
    public class BaseResponse
    {
        [JsonProperty("is_valid")]
        public bool IsValid { get; set; } = true;

        [JsonProperty("is_not_found", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsNotFound { get; set; }

        [JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Errors { get; set; }

        /// <summary>
        /// A helper method to provide JsonResponse with HttpStatusCode based on IsValid property value
        /// </summary>
        /// <param name="overrideStatusCode"></param>
        /// <returns></returns>
        public Microsoft.AspNetCore.Mvc.JsonResult JsonResult(System.Net.HttpStatusCode? overrideStatusCode = null)
        {
            var jsonResult = new Microsoft.AspNetCore.Mvc.JsonResult(this);
            if (overrideStatusCode.HasValue)
            {
                jsonResult.StatusCode = (int)(overrideStatusCode.Value);
            }
            else
            {
                if (this.IsNotFound.HasValue && this.IsNotFound.Value)
                {
                    jsonResult.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
                }
                else
                {
                    jsonResult.StatusCode = (int)(this.IsValid ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.BadRequest);
                }
                

            }
            return jsonResult;
        }

    }
}
