using APP.Data.Modelos;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;
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

		public static async Task<RespuestaConsumidor<Tout>> Execute<Tin, Tout>(
				string url, MethodHttp method, Tin 
				objectRequest, ProtectedLocalStorage protectedLocalStorage = null, 
				bool habiaMasDatosEnQuery = false)
		{

			RespuestaConsumidor<Tout> respuesta = new RespuestaConsumidor<Tout>();
			try
			{
				using (HttpClient client = new HttpClient())
				{
					var myContent = JsonConvert.SerializeObject((method != MethodHttp.GET) ? method != MethodHttp.DELETE ? objectRequest : "" : "");
					var bytecontent = new ByteArrayContent(Encoding.UTF8.GetBytes(myContent));
					bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
					string newUrl = url;

					//esto es para pasar el url con el id del usuario sesion en el path, ya que siempre que hacemos esto es cuando necesitamos el token
					//entonces verifico si se pasó el localstorage para validar esto.
					if (protectedLocalStorage != null)
					{
						var jwt = await protectedLocalStorage.GetAsync<int>("idusuariosesion");
						int idusuario = jwt.Success ? jwt.Value : 0;

						//Esto es por si cuando se pasa el url, resulta que se había agregado otro dato en el query, entonces para que no haya conflictos en la url agregué esa validación
						if (habiaMasDatosEnQuery)
						{
							newUrl += $"&idusuariosesion={idusuario}";
						}
						else
						{
							newUrl += $"?idusuariosesion={idusuario}";
						}						
					}

					var request = new HttpRequestMessage(CreateHttpMethod(method), newUrl)
					{
						Content = (method != MethodHttp.GET) ? method != MethodHttp.DELETE ? bytecontent : null : null
					};

					//Igual, si pasan esto es porque es un método que requiere autorización.
					if (protectedLocalStorage != null)
					{
						var jwt = await protectedLocalStorage.GetAsync<string>("jwt");
						string token = jwt.Success ? jwt.Value : "";

						request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
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
									if (typeof(Tout) == typeof(string)) //JsonConvert tonto da error al deserializar strings así que esto es por si acaso
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
                                if (res.StatusCode == HttpStatusCode.BadRequest || res.StatusCode == HttpStatusCode.Unauthorized)
								{
                                    
                                    if (data != null)
                                    {
										try
										{
                                            var mensajeerror = JsonConvert.DeserializeObject<MensajeErrorAPI>(data);
                                            respuesta.Mensaje = mensajeerror.message;
                                        }
										catch
										{
                                            respuesta.Mensaje = $"Ocurrió un error no controlado en la API (perdon)\n{data}";
                                        }
                                        
                                        
                                    }
                                }
								else //este sería un error no controlado de la API
								{
									respuesta.Mensaje = $"Ocurrió un error no controlado en la API (perdon)\n{data}";
								}


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
				if (!respuesta.Ok)
				{
					respuesta.Mensaje = $"Error en el servidor.\nStatus code: {respuesta.StatusCode}"; //asd
				}
			}
			catch(HttpRequestException ex)
			{
                respuesta.StatusCode = "Error conexión";
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }
			catch (JsonSerializationException) 
			{
				//
			}
			catch (CryptographicException ex) //Este excepción sucedía cuando alguien cambia manualmente algo en el local storage y la librería no puede descifrar los datos.
			{ //me di cuenta que cualquier problema al serializar puede suceder, ni modo.
				if (protectedLocalStorage != null)
				{
                    await protectedLocalStorage.DeleteAsync("jwt");
                    await protectedLocalStorage.DeleteAsync("idusuariosesion");
                }
				
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
