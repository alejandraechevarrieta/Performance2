using Performance.Model;
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
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Performance.Areas.Login.Controllers.Api
{
    public class LoginController : ApiController
    {
        [System.Web.Http.Route("Api/Login/ValidarIngreso")]
        [System.Web.Http.ActionName("ValidarIngreso")]
        [System.Web.Http.HttpGet]
        public async Task<int> ValidarIngreso(string usuario, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var idUsuario = GetUsuario(usuario, password);
                    if (idUsuario > 0)
                    {
                        return idUsuario;
                    }
                    else
                    {
                        Console.WriteLine("La respuesta no contiene un formato válido.");
                        return 0;
                    }

                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine("Error al leer la respuesta JSON:");
                    Console.WriteLine(ex.Message);
                    throw;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("Error al realizar la solicitud HTTP:");
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        [System.Web.Http.Route("api/Login/GetPerfiles")]
        [System.Web.Http.ActionName("GetPerfiles")]
        [System.Web.Http.HttpGet]
        public int GetPerfiles(int idUsuario)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Ejecuta la tarea de forma sincrónica dentro de un Task.Run
                    var perfiles = GetPerfiless(idUsuario);

                    if (perfiles != null) { 
                        var perfil = 0;

                        //asigno perfil 
                        foreach (var item in perfiles)
                        {

                            if (item.idPerfil == 127 || item.idPerfil == 128 || item.idPerfil == 129)
                            {
                                perfil = Convert.ToInt32(item.idPerfil);
                                break;
                            }
                        }
                        return perfil;
                    }
                    else
                    {
                        Console.WriteLine("La respuesta JSON está vacía.");
                        return 0; // Retorna 0 en caso de que la respuesta esté vacía
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("Error al realizar la solicitud HTTP:");
                    Console.WriteLine(ex.Message);
                    return 0; // Retorna 0 en caso de error HTTP
                }
                catch (JsonSerializationException ex)
                {
                    Console.WriteLine("Error al deserializar la respuesta JSON:");
                    Console.WriteLine(ex.Message);
                    return 0; // Retorna 0 en caso de error de deserialización
                }
            }
        }

        [System.Web.Http.Route("api/Login/GetUsuario")]
        [System.Web.Http.ActionName("GetUsuario")]
        [System.Web.Http.HttpGet]
        public int GetUsuario(string usuario, string password)
        {
            using (BuhoGestionEntities db = new BuhoGestionEntities())
            {
                if (usuario != null && password != null)
                {
                    var tmp = db.GralUsuario.Where(x => x.usuario == usuario).FirstOrDefault();
                    Dictionary<string, int> loginResult = null;
                    var result = 0;
                    if (tmp != null)
                    {                       
                        if (tmp.activo == 1)
                        {   
                            if (password != tmp.password) //contraseña incorrecta
                            {
                                result = 0;
                                return (result);
                            }
                            else
                            {
                                result = tmp.idUsuario;
                                return (result); ;
                            }
                        }
                    }
                    result = -1; //usuario no activo
                }
            }
            return (0);
        }
       
        [System.Web.Http.Route("api/Login/GetPerfiless")]
        [System.Web.Http.ActionName("GetPerfiless")]
        [System.Web.Http.HttpGet]
        public List<PerfilesVM> GetPerfiless(int idUsuario)
        {

            using (BuhoGestionEntities db = new BuhoGestionEntities())
            {
                var tmp = (from p in db.SegUsuarioPerfil
                           where p.idUsuario == idUsuario
                           select new PerfilesVM
                           {
                               idPerfil = p.idPerfil,

                           });
                var algo = tmp.ToList();
                var temp = (IQueryable<PerfilesVM>)tmp;
                temp = temp.Union(temp);
                return algo;
            }
        }

        [System.Web.Http.Route("api/Login/DatosColaboradores")]
        [System.Web.Http.ActionName("DatosColaboradores")]
        [System.Web.Http.HttpGet]      
            public List<object> DatosColaboradores()
            {
                using (BuhoGestionEntities db = new BuhoGestionEntities())
                {
                    var datosColaboradores = (from colaborador in db.GralUsuario
                                              join jefe in db.GralUsuario
                                              on colaborador.idJefe equals jefe.idUsuario into jefes
                                              from jefe in jefes.DefaultIfEmpty()
                                              join c in db.UocCategoria
                                              on colaborador.categoria equals c.idCategoria into cc
                                              from c in cc.DefaultIfEmpty()
                                              join co in db.RecConvenio
                                              on colaborador.idConvenio equals co.idConvenio into con
                                              from co in con.DefaultIfEmpty()
                                              join d in db.GralDominio
                                              on colaborador.idDominio equals d.idDominio into dom
                                              from d in dom.DefaultIfEmpty()
                                              where colaborador.activo == 1
                                                    && colaborador.Nombre != null
                                                    && colaborador.idJefe != null && colaborador.idJefe != 0
                                                    && colaborador.fechaIngreso != null
                                                    && colaborador.esPerformance == true
                                              select new
                                              {
                                                  idUsuario = colaborador.idUsuario,
                                                  legajo = colaborador.nroSAP,
                                                  nombre = colaborador.Nombre,
                                                  idJefe = colaborador.idJefe,
                                                  nombreJefe = jefe != null ? jefe.Nombre : null,
                                                  fechaIngreso = colaborador.fechaIngreso,
                                                  fechaNacimiento = colaborador.fechaNacimiento,
                                                  pais = colaborador.pais,
                                                  sexo = colaborador.sexo,
                                                  categoria = c.nombre,
                                                  convenio = co.nombreConvenio,
                                                  dominio = d.NombreDominio,
                                              }).ToList();

                    return datosColaboradores.Cast<object>().ToList();
                }
            }
        [System.Web.Http.Route("api/Login/DatosUsuario")]
        [System.Web.Http.ActionName("DatosUsuario")]
        [System.Web.Http.HttpGet]      
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

        }
}
