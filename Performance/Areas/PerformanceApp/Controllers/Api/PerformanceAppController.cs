﻿using Performance.Model;
using Performance.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Web.Hosting;
using System.Net.Http.Headers;
using System.Web.Services.Description;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Performance.Areas.PerformanceApp.Controllers.Api
{
    public class PerformanceAppController : ApiController
    {
        //public ActionResult Index()
        //{
        //    return View();
        //}
        //public List<PerformanceColaboradorVM> listarColaborador(int idUsuario)
        //{
        //    ServicioPerformance servicio = new ServicioPerformance();
        //    return servicio.listarColaboradores(idUsuario);
        //}
        [System.Web.Http.Route("Api/PerformanceApp/listarColaborador")]
        [System.Web.Http.ActionName("listarColaborador")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage listarColaborador(FormDataCollection form, int? colaborador, int? estado, string idPerfil)
        {
            //List<PerformanceColaboradorVM>


            //var draw = form.Get("draw");
            //var start = form.Get("start");
            //var length = form.Get("length");
            //var sortColumn = (form.Get("columns[" + form.Get("order[0][column]").FirstOrDefault() + "][data]").ToString()).ToString();
            //var sortColumnDir = form.Get("order[0][dir]").ToString();
            //var searchValue = form.Get("search[value]").ToString();
            int pageSize = 25;
            int skip = 1;
            int recordsTotal = 0;

            Servicios.ServicioPerformance servicio = new Servicios.ServicioPerformance();

            var listaPpal = servicio.listarColaboradores(colaborador);

            var listFiltr = listaPpal.Where(x => x.idPerformance > 0).Distinct().ToList();

            recordsTotal = listFiltr.Count();
            var toTake = pageSize;
            if (recordsTotal < pageSize)
            {
                toTake = recordsTotal;
            }

            var lst = listFiltr.Skip(skip).Take(toTake).ToList();
            var lista = lst.ToList();

            var responseData = new { draw = "", recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = lista };

            // Serialize responseData to JSON
            var jsonResult = JsonConvert.SerializeObject(responseData);

            // Create an HttpResponseMessage with the JSON content
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");

            // Return the HttpResponseMessage
            return response;
        }
        [System.Web.Http.Route("Api/PerformanceApp/ListarPerformance")]
        [System.Web.Http.ActionName("ListarPerformance")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage ListarPerformance(DataTableRequestModel requestModel, string idUsuario, string idPerfil, int? colaborador, int? estado, int? lider, int? ano, string dominio, string convenio)
        {

            var draw = requestModel.Draw;
            var start = requestModel.Start;
            var length = requestModel.Length;
            var sortColumn = requestModel.Columns[Convert.ToInt32(requestModel.Order[0].Column)].Data;
            var sortColumnDir = requestModel.Order[0].Dir;
            var searchValue = requestModel.Search.Value;

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();

            // Consulta a tu servicio para obtener los datos
            var listaPpal = _servicio.listarPerformance(searchValue, sortColumn, sortColumnDir, idUsuario, idPerfil, colaborador, estado, lider, ano, dominio, convenio);

            // Filtrar y paginar los datos según los parámetros recibidos
            var listFiltr = listaPpal.Where(x => x.idPerformance > 0).Distinct().ToList();

            recordsTotal = listFiltr.Count();
            var toTake = pageSize;
            if (recordsTotal < pageSize)
            {
                toTake = recordsTotal;
            }

            var lst = listFiltr.Skip(skip).Take(toTake).ToList();
            var lista = lst.ToList();

            var responseData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = lista };

            // Serialize responseData to JSON
            var jsonResult = JsonConvert.SerializeObject(responseData);

            // Create an HttpResponseMessage with the JSON content
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");

            // Return the HttpResponseMessage
            return response;
        }

        [System.Web.Http.Route("Api/PerformanceApp/buscarPerformance")]
        [System.Web.Http.ActionName("buscarPerformance")]
        [System.Web.Http.HttpGet]
        public PerformanceVM buscarPerformance(int idPerformance)
        {
            Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();

            var performance = _servicio.buscarPerformance(idPerformance);
            return performance;
        }

        [System.Web.Http.Route("Api/PerformanceApp/listarEstadosPerformance")]
        [System.Web.Http.ActionName("listarEstadosPerformance")]
        [System.Web.Http.HttpGet]
        public List<PerformanceEstados> listarEstadosPerformance()
        {
            Servicios.ServicioPerformance servicio = new Servicios.ServicioPerformance();
            return servicio.listarEstadosPerformance();
        }
        [System.Web.Http.Route("Api/PerformanceApp/listarAnosPerformance")]
        [System.Web.Http.ActionName("listarAnosPerformance")]
        [System.Web.Http.HttpGet]
        public List<PerformanceAnos> listarAnosPerformance()
        {
            Servicios.ServicioPerformance servicio = new Servicios.ServicioPerformance();
            return servicio.listarAnosPerformance();
        }
        [System.Web.Http.Route("/ListarColaboradores/")]
        [System.Web.Http.ActionName("ListarColaboradores")]
        [System.Web.Http.HttpGet]
        public List<ColaboradorVM> ListarColaboradores()
        {
            Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();
            var tmp = _servicio.ListarColaboradores();
            return tmp;
        }
        [System.Web.Http.Route("/ListarLideres/")]
        [System.Web.Http.ActionName("ListarLideres")]
        [System.Web.Http.HttpGet]
        public List<ColaboradorVM> ListarLideres()
        {
            Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();
            var tmp = _servicio.ListarLideres();
            return tmp;
        }
        [System.Web.Http.Route("/ListarDominios/")]
        [System.Web.Http.ActionName("ListarDominios")]
        [System.Web.Http.HttpGet]
        public List<ColaboradorVM> ListarDominios()
        {
            Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();
            var tmp = _servicio.ListarDominios();
            return tmp;
        }
        [System.Web.Http.Route("/ListarConvenios/")]
        [System.Web.Http.ActionName("ListarConvenios")]
        [System.Web.Http.HttpGet]
        public List<ColaboradorVM> ListarConvenios()
        {
            Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();
            var tmp = _servicio.ListarConvenios();
            return tmp;
        }
        [System.Web.Http.Route("Api/PerformanceApp/listarHistorial")]
        [System.Web.Http.ActionName("listarHistorial")]
        [System.Web.Http.HttpGet]
        public List<HistorialVM> listarHistorial(int idPerformance, int idUsuario)
        {
            Servicios.ServicioPerformance servicio = new Servicios.ServicioPerformance();
            return servicio.ListarHistorial(idPerformance, idUsuario);
        }
        [System.Web.Http.Route("Api/PerformanceApp/GuardarAutoevaluacion")]
        [System.Web.Http.ActionName("GuardarAutoevaluacion")]
        [System.Web.Http.HttpPost]
        public async Task<int> GuardarAutoevaluacion(PerformanceAutoevaluacionVM autoevaluacion)
        {
            try
            {
                using (var db = new PerformanceEntities())
                {
                    Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();

                    var tmp = _servicio.GuardarAutoevaluacion(autoevaluacion);

                    var performance = db.PerformanceColaborador.Where(x => x.idPerformance == autoevaluacion.idPerformance).FirstOrDefault();

                    await EnviarMailLider(performance.idUsuario, performance.idJefe, tmp.body, tmp.asunto);
                    
                    return tmp.idPerformance;
                }
                
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        [System.Web.Http.Route("Api/PerformanceApp/EliminarPerformance")]
        [System.Web.Http.ActionName("EliminarPerformance")]
        [System.Web.Http.HttpPost]
        public int EliminarPerformance(PerformanceAutoevaluacionVM objeto)
        {
            try
            {
                using (var db = new PerformanceEntities())
                {
                    Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();

                    var tmp = _servicio.EliminarPerformance(objeto.idPerformance, objeto.idUsuario, objeto.nombreUsuario);

                    //var performance = db.PerformanceColaborador.Where(x => x.idPerformance == autoevaluacion.idPerformance).FirstOrDefault();

                    //await EnviarMailLider(performance.idUsuario, performance.idJefe, tmp.body, tmp.asunto);

                    return tmp.idPerformance;
                }

            }
            catch (Exception e)
            {
                return 0;
            }
        }
        [System.Web.Http.Route("Api/PerformanceApp/GuardarEvaluacion")]
        [System.Web.Http.ActionName("GuardarEvaluacion")]
        [System.Web.Http.HttpPost]
        public async Task<int> GuardarEvaluacion(PerformanceAutoevaluacionVM evaluacion)
        {
            try
            {
                using (var db = new PerformanceEntities())
                {
                    Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();

                    var tmp = _servicio.GuardarEvaluacion(evaluacion, evaluacion.idResponsable);

                    var performance = db.PerformanceColaborador.Where(x => x.idPerformance == evaluacion.idPerformance).FirstOrDefault();

                    //await EnviarMailLider(performance.idUsuario, performance.idJefe, tmp.body, tmp.asunto);

                    return tmp.idPerformance;
                }

            }
            catch (Exception e)
            {
                return 0;
            }
        }
        [System.Web.Http.Route("Api/PerformanceApp/GuardarCalibracion")]
        [System.Web.Http.ActionName("GuardarCalibracion")]
        [System.Web.Http.HttpPost]
        public async Task<int> GuardarCalibracion(PerformanceCalibracionVM calibracion)
        {
            try
            {
                using (var db = new PerformanceEntities())
                {
                    Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();

                    var tmp = _servicio.GuardarCalibracion(calibracion);
                    return tmp.idPerformance;
                }

            }
            catch (Exception e)
            {
                return 0;
            }
        }

        [System.Web.Http.Route("Api/PerformanceApp/GenerarAltasPerformance")]
        [System.Web.Http.ActionName("GenerarAltasPerformance")]
        [System.Web.Http.HttpGet]
       public async Task<int> GenerarAltasPerformance()
        {
            string API_BASE_URL = ConfigurationSettings.AppSettings["urlBuho"];
            string endpoint = $"Ingenieria/api/Login/DatosColaboradores";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(API_BASE_URL + endpoint);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Deserializa la respuesta JSON directamente a una lista de objetos ColaboradorVM
                    List<ColaboradorVM> colaboradores = JsonConvert.DeserializeObject<List<ColaboradorVM>>(responseBody);

                    Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();
                    var tmp = _servicio.GenerarAltasPerformance(colaboradores.ToList());

                    return tmp;
                }
                catch (JsonSerializationException ex)
                {
                    Console.WriteLine("Error al deserializar la respuesta JSON:");
                    Console.WriteLine(ex.Message);
                    throw;                    
                }
            }
        }
       
        [System.Web.Http.Route("Api/PerformanceApp/BuscarDatosUsuario")]
        [System.Web.Http.ActionName("BuscarDatosUsuario")]
        [System.Web.Http.HttpGet]
        public object BuscarDatosUsuario(int idUsuario)
        {    
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var colaborador = DatosUsuario(idUsuario);
                    return colaborador;
                }
                catch (JsonSerializationException ex)
                {
                    Console.WriteLine("Error al deserializar la respuesta JSON:");
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }
       
        public object DatosUsuario(int idUsuario)
        {
            using (BuhoGestionEntities db = new BuhoGestionEntities())
            {
                var datosColaborador = (from colaborador in db.GralUsuario
                                        join jefe in db.GralUsuario
                                        on colaborador.idJefe equals jefe.idUsuario into jefes
                                        from jefe in jefes.DefaultIfEmpty()
                                        where colaborador.idUsuario == idUsuario
                                        select new
                                        {
                                            idUsuario = colaborador.idUsuario,
                                            nombre = colaborador.Nombre,
                                            idJefe = colaborador.idJefe,
                                            nombreJefe = jefe != null ? jefe.Nombre : null,
                                            fechaIngreso = colaborador.fechaIngreso
                                        }).FirstOrDefault();

                return datosColaborador;
            }
        }

        /// <summary>
        /// listar performance para progreso 
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.Route("Api/PerformanceApp/listarPerformanceProgreso")]
        [System.Web.Http.ActionName("listarPerformanceProgreso")]
        [System.Web.Http.HttpGet]
        public List<PerformanceProgresoVM> listarPerformanceProgreso()
        {
            Servicios.ServicioPerformance servicio = new Servicios.ServicioPerformance();
            return servicio.listarPerformanceProgreso();
        }     
        [System.Web.Http.Route("Api/PerformanceApp/buscarDatosPerformance")]
        [System.Web.Http.ActionName("buscarDatosPerformance")]
        [System.Web.Http.HttpGet]
        public DatosGeneralPerformanceVM buscarDatosPerformance(int idPerformance, int perfil)
        {
            Servicios.ServicioPerformance servicio = new Servicios.ServicioPerformance();
            return servicio.buscarDatosPerformance(idPerformance, perfil);
        }

        [System.Web.Http.Route("Api/PerformanceApp/EnviarMailLider")]
        [System.Web.Http.ActionName("EnviarMailLider")]
        [System.Web.Http.HttpGet]
        public async Task<string> EnviarMailLider(int idUsuario, int idDestinatario, string body, string asunto)
        {
            var token = GenerarToken();
            await EnviarMailAsync(token, idUsuario, idDestinatario, body, asunto);

            return token.ToString();
        }

        [System.Web.Http.Route("Api/PerformanceApp/GenerarToken")]
        [System.Web.Http.ActionName("GenerarToken")]
        [System.Web.Http.HttpGet]
        public string GenerarToken()
        {
            using (var client = new HttpClient())
            {
                TokenRequest request = new TokenRequest
                {
                    ClientId = "performance",
                    ClientSecret = "envio_mail"
                };

                string API_BASE_URL = ConfigurationSettings.AppSettings["urlBuho"];
                client.BaseAddress = new Uri(API_BASE_URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                // Realiza la solicitud de forma síncrona
                var response = client.PostAsync("AuthToken/api/AuthToken/GenerateToken", content).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                // Lee el contenido de la respuesta como una cadena de forma síncrona
                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                // Deserializa el contenido de la respuesta a una instancia de TokenResponse
                TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                // Obtén el valor del token
                string token = tokenResponse.Token;
                return token;
            }
        }

        [System.Web.Http.Route("Api/PerformanceApp/EnviarMailAsync")]
        [System.Web.Http.ActionName("EnviarMailAsync")]
        [System.Web.Http.HttpGet]
        static async Task EnviarMailAsync(string token, int idUsuario, int idDestinatario, string body, string asunto)
        {
            using (var client = new HttpClient())
            {
                string API_BASE_URL = ConfigurationSettings.AppSettings["urlBuho"];
                client.BaseAddress = new Uri(API_BASE_URL); // Reemplaza con la URL de tu API
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Crea el contenido del cuerpo de la solicitud
                var content = new
                {
                    IdDestinatario = idDestinatario
                    // Agrega aquí más propiedades si son necesarias
                };

                // Inicializa la instancia de MailVM y su lista IdsDestinatarios
                MailVM mail = new MailVM
                {
                    IdsDestinatarios = new List<int> { idDestinatario },
                    mensaje = body, // Reemplaza con el mensaje adecuado
                    asunto = asunto   // Reemplaza con el asunto adecuado
                    //adjunto = "Tu adjunto aquí"  // Reemplaza con el adjunto adecuado
                };
                // Serializa el objeto MailVM a JSON
                var jsonContent = JsonConvert.SerializeObject(mail);

                // Crea un StringContent a partir del JSON
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"EmailExp/api/EmailExp/EnviarMailGenerico", httpContent);
                response.EnsureSuccessStatusCode();

                // Aquí puedes procesar la respuesta si es necesario
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
            }
        }

        //EXCEL DE AUTOEVALUACION
        [System.Web.Http.Route("/GenerarExcelUnColaborador")]
        [System.Web.Http.ActionName("GenerarExcelUnColaborador")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GenerarExcelUnColaborador(int idPerformance)
        {
            Servicios.ServicioPerformance servicio = new Servicios.ServicioPerformance();
            var excel = servicio.GenerarExcelUnColaborador(idPerformance);

            return DownloadFile(excel.filePath, excel.fileName);
        }

        public class TokenRequest
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }

        [System.Web.Http.ActionName("DescargarInstructivo")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult DescargarInstructivo()
        {
            HttpResponseMessage result = null;
            try
            {
                string filePath = "D:\\Archivos\\Performance\\InstructivoPerformance.pdf";
                IHttpActionResult response;
                HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.OK);
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                responseMsg.Content = new StreamContent(fileStream);
                responseMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                responseMsg.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                responseMsg.Content.Headers.ContentDisposition.FileName = "InstructivoPerformance.pdf";
                response = ResponseMessage(responseMsg);

                return response;


            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        /// <summary>
        /// REPORTES
        /// </summary>
        [System.Web.Http.Route("Api/Viaticos/GenerarExcelReportesColaboradores")]
        [System.Web.Http.ActionName("GenerarExcelReportesColaboradores")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GenerarExcelReportesColaboradores(int? colaborador, int? estado, int? lider, int? ano)
        {

            Servicios.ServicioPerformance servicio = new Servicios.ServicioPerformance();
            var excel = servicio.GenerarExcelReportesColaboradores(colaborador, estado, lider, ano);
                return DownloadFile(excel.filePath, excel.fileName);
            
        }
        private HttpResponseMessage DownloadFile(string downloadFilePath, string fileName)
        {
            try
            {
                //Check if the file exists. If the file doesn't exist, throw a file not found exception
                if (!System.IO.File.Exists(downloadFilePath))
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                //Copy the source file stream to MemoryStream and close the file stream
                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = System.IO.File.Open(downloadFilePath, FileMode.Open);

                fileStream.CopyTo(responseStream);
                fileStream.Close();
                responseStream.Position = 0;

                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.OK;

                //Write the memory stream to HttpResponseMessage content
                response.Content = new StreamContent(responseStream);
                string contentDisposition = string.Concat("attachment; filename=", fileName);
                response.Content.Headers.ContentDisposition =
                              ContentDispositionHeaderValue.Parse(contentDisposition);
                return response;
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
        [System.Web.Http.Route("Api/PerformanceApp/CambiarEstado")]
        [System.Web.Http.ActionName("CambiarEstado")]
        [System.Web.Http.HttpGet]
        public void CambiarEstado(int idPerformance, int estado, int idUsuario, string nombreUsuario)
        {
            Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();
            _servicio.CambiarEstado(idPerformance, estado, idUsuario, nombreUsuario);            
        }
        [System.Web.Http.Route("Api/PerformanceApp/EstadoActual")]
        [System.Web.Http.ActionName("EstadoActual")]
        [System.Web.Http.HttpGet]
        public int EstadoActual()
        {
            Servicios.ServicioPerformance servicio = new Servicios.ServicioPerformance();
            return servicio.EstadoActual();
        }
        [System.Web.Http.Route("Api/PerformanceApp/buscarDatosPDI")]
        [System.Web.Http.ActionName("buscarDatosPDI")]
        [System.Web.Http.HttpGet]
        public List<PDIMetasVM> buscarDatosPDI(int idUsuario, int idPerformance)
        {
            Servicios.ServicioPerformance servicio = new Servicios.ServicioPerformance();
            return servicio.buscarDatosPDI(idUsuario, idPerformance);
        }
        [System.Web.Http.Route("/ListarTipoAccion/")]
        [System.Web.Http.ActionName("ListarTipoAccion")]
        [System.Web.Http.HttpGet]
        public List<TipoAccionVM> ListarTipoAccion()
        {
            Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();
            var tmp = _servicio.ListarTipoAccion();
            return tmp;
        }
        [System.Web.Http.Route("/ListarStatus/")]
        [System.Web.Http.ActionName("ListarStatus")]
        [System.Web.Http.HttpGet]
        public List<StatusVM> ListarStatus()
        {
            Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();
            var tmp = _servicio.ListarStatus();
            return tmp;
        }
        [System.Web.Http.Route("/ListarHabilidades/")]
        [System.Web.Http.ActionName("ListarHabilidades")]
        [System.Web.Http.HttpGet]
        public List<HabilidadesVM> ListarHabilidades()
        {
            Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();
            var tmp = _servicio.ListarHabilidades();
            return tmp;
        }
        [System.Web.Http.Route("Api/PerformanceApp/GuardarMeta")]
        [System.Web.Http.ActionName("GuardarMeta")]
        [System.Web.Http.HttpPost]
        public int GuardarMeta(PDIMetasVM meta)
        {
            try
            {
                using (var db = new PerformanceEntities())
                {
                    Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();

                    var tmp = _servicio.GuardarMeta(meta);
                
                    return tmp;
                }

            }
            catch (Exception e)
            {
                return 0;
            }
        }
        [System.Web.Http.Route("Api/PerformanceApp/buscarDatosMeta")]
        [System.Web.Http.ActionName("buscarDatosMeta")]
        [System.Web.Http.HttpGet]
        public PDIMetasVM buscarDatosMeta(int idUsuario, int idMeta)
        {
            Servicios.ServicioPerformance servicio = new Servicios.ServicioPerformance();
            return servicio.buscarDatosMeta(idUsuario, idMeta);
        }
        [System.Web.Http.Route("Api/PerformanceApp/cambiarFeedback")]
        [System.Web.Http.ActionName("cambiarFeedback")]
        [System.Web.Http.HttpGet]
        public void cambiarFeedback(int idUsuario, string nombreUsuario)
        {
            Servicios.ServicioPerformance _servicio = new Servicios.ServicioPerformance();
            _servicio.cambiarFeedback(idUsuario, nombreUsuario);
        }
        public class DataTableRequestModel
        {
            public int Draw { get; set; }
            public int Start { get; set; }
            public int Length { get; set; }
            public List<Column> Columns { get; set; }
            public List<Order> Order { get; set; }
            public Search Search { get; set; }
            public int? colaborador { get; set; }
            public int? estado { get; set; }
            public int idPerfil { get; set; }
        }       

        public class TokenResponse
        {
            public string Token { get; set; }
        }
       

        public class Column
        {
            public string Data { get; set; }
        }

        public class Order
        {
            public int Column { get; set; }
            public string Dir { get; set; }
        }

        public class Search
        {
            public string Value { get; set; }
        }

    }
}
