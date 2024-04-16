using APP.Data.Modelos;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace APP.Data
{
	public class Consumidor
	{

		public readonly ProtectedLocalStorage _protectedLocalStorage;
		public Consumidor(ProtectedLocalStorage protectedLocalStorage)
		{
			_protectedLocalStorage = protectedLocalStorage;
		}

		private static HttpMethod CreateHttpMethod(MethodHttp method)
		{
			switch (method)
			{
				case MethodHttp.GET:
					return HttpMethod.Get;
				case MethodHttp.POST:
					return HttpMethod.Post;
				case MethodHttp.PUT:
					return HttpMethod.Put;
				case MethodHttp.DELETE:
					return HttpMethod.Delete;
				default:
					throw new NotImplementedException("Not implemented http method");
			}
		}

		public static async Task<Respuesta<Tout>> Execute<Tin, Tout>(string url, MethodHttp method, Tin objectRequest, string jwt = "")
		{

			Respuesta<Tout> respuesta = new Respuesta<Tout>();
			try
			{
				using (HttpClient client = new HttpClient())
				{

					var myContent = JsonConvert.SerializeObject((method != MethodHttp.GET) ? method != MethodHttp.DELETE ? objectRequest : "" : "");
					//var myContent = JsonConvert.SerializeObject(objectRequest);
					var bytecontent = new ByteArrayContent(Encoding.UTF8.GetBytes(myContent));

					bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

					//Si es get o delete no le mandamos bytecontent. Tremenda línea.
					var request = new HttpRequestMessage(CreateHttpMethod(method), url)
					{
						//Por regla general de las peticiones HTTP las peticiones tipo GET y DELETE no se le puede establecer el body
						//Entonces valido, si method es distinta de GET y DELETE le asigno el contenido codificado, sino le asigno null
						Content = (method != MethodHttp.GET) ? method != MethodHttp.DELETE ? bytecontent : null : null
					};

					if (jwt != "")
					{
						request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
					}

					using (HttpResponseMessage res = await client.SendAsync(request))
					{

						using (HttpContent content = res.Content)
						{
							string data = await content.ReadAsStringAsync();

							respuesta.StatusCode = res.StatusCode.ToString();

							if (res.IsSuccessStatusCode)
							{
								respuesta.Ok = true;

								if (data != null)
								{
									if (typeof(Tout) == typeof(string)) //JsonConvert tonto da error al deserializar strings
									{
										respuesta.Data = (Tout)Convert.ChangeType(data, typeof(Tout));
									}
									else
									{
										respuesta.Data = JsonConvert.DeserializeObject<Tout>(data);
									}
								}
							}
							else
							{
								respuesta.Ok = false;
								var mensajeerror = JsonConvert.DeserializeObject<MensajeErrorAPI>(data);
								respuesta.Mensaje = mensajeerror.message;
							}



						}
					}
				}
			}
			catch (WebException ex)
			{
				respuesta.StatusCode = "ServerError";
				var res = (HttpWebResponse)ex.Response;
				if (res != null)
					respuesta.StatusCode = respuesta.StatusCode.ToString();
				respuesta.Ok = false; //?
				if (!respuesta.Ok) respuesta.Mensaje = $"Response not OK\nStatus code: {respuesta.StatusCode}"; //asd
			}
			catch (JsonSerializationException) 
			{
			
				respuesta.StatusCode = "Token invalid";
				respuesta.Mensaje = "Token invalid error, sign in again";
				respuesta.Ok = false;
			}
			catch (Exception ex)
			{
				respuesta.StatusCode = "App error";
				respuesta.Mensaje = ex.Message;
				respuesta.Ok = false;
			}

			return respuesta;


		}

	}
}
