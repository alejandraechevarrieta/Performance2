﻿using Performance.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Performance.Servicios
{
    public class ServicioPerformance : IDisposable
    {
        private readonly PerformanceEntities db;

        public ServicioPerformance()
        {
            db = new PerformanceEntities(); 
        }

        public List<PerformanceColaboradorVM> listarColaboradores(int? idUsuario)
        {
            var query = (from d in db.PerformanceColaborador
                         select new PerformanceColaboradorVM
                         {
                             idPerformance = d.idPerformance,
                             idUsuario = d.idUsuario,
                             nombre = d.nombre,
                             ano = d.ano,
                             antiguedad = d.antiguedad,
                             idJefe = d.idJefe,
                         }).ToList();
            query = query.Where(x => x.idUsuario == idUsuario).ToList();
            return query;
        }

        public IQueryable<PerformanceVM> ListarPerformanceTodas()
        {
            var tmp = (from p in db.PerformanceColaborador
                       join e in db.Estados on p.estado equals e.id
                       select new PerformanceVM
                       {
                           ano = p.ano,
                           idPerformance = p.idPerformance,
                           idUsuario = p.idUsuario,
                           nombre = p.nombre,
                           idJefe = p.idJefe,
                           nombreJefe = p.nombreJefe,
                           antiguedad = p.antiguedad,
                           fechaAutoevaluacion = p.fechaAutoevaluacion, 
                           fechaEvaluacion = p.fechaEvaluacion,
                           fechaCalibracion = p.fechaCalibracion,
                           fechaFeedback = null, //cambiar
                           idEstado = p.estado,
                           estado = e.estado,
                           dominio = p.dominio,
                           eliminado = p.eliminado,
                           convenio = p.convenio,
                       }).Where(x => x.eliminado != true).OrderByDescending(x => x.ano).ThenBy(x => x.nombre);
            var algo = tmp.ToList();
            var temp = (IQueryable<PerformanceVM>)tmp;
            temp = temp.Union(temp);
            return temp;
        }
        public List<PerformanceVM> listarPerformance(string filtro, string orden, string direccion, string idUsuario, string idPerfil, int? colaborador, int? estado, int? lider, int? ano, string dominio, string convenio)
        {
            var listaPpal = ListarPerformanceTodas().ToList();
            var idUsuarioInt = Convert.ToInt16(idUsuario);

            if (idPerfil != null)
            {
                if (idPerfil == "127")
                {
                    listaPpal = listaPpal.Where(p => p.idUsuario == idUsuarioInt).ToList();
                }
                if (idPerfil == "128")
                {
                    listaPpal = listaPpal.Where(p => p.idJefe == idUsuarioInt).ToList();
                }
            }
            if (colaborador != null)
            {
                listaPpal = listaPpal.Where(p => p.idUsuario == colaborador).ToList();
            }
            if (estado != null)
            {
                listaPpal = listaPpal.Where(p => p.idEstado == estado).ToList();
            }
            if (lider != null)
            {
                listaPpal = listaPpal.Where(p => p.idJefe == lider).ToList();
            }
            if (ano != null)
            {
                listaPpal = listaPpal.Where(p => p.ano == ano).ToList();
            }
            if (dominio != "null" && dominio != null)
            {
                var dominioNormalizado = normalizarTexto(dominio).Substring(0, 3);

                listaPpal = listaPpal.Where(p => p.dominio != "null" &&
                                                  normalizarTexto(p.dominio).Substring(0, 3) == dominioNormalizado).ToList();
            }
            if (convenio != "null" && convenio != null)
            {
                var convenioNormalizado = normalizarTexto(convenio).Substring(0, 2);

                listaPpal = listaPpal.Where(p => p.convenio != null &&
                                                  normalizarTexto(p.convenio).Substring(0, 2) == convenioNormalizado).ToList();
            }           
            var temp = listaPpal.AsQueryable();
            //filtro
            if (!string.IsNullOrEmpty(filtro))
            {
                string filtroLower = filtro.ToLower(); // Convertir filtro a minúsculas

                temp = temp.Where(x => x.nombre.ToLower().Contains(filtroLower));
            }

            //orden

            if (orden != null)
            {
                if (direccion == "desc")
                {
                    switch (orden)
                    {
                        case "nombre":
                            temp = temp.OrderBy(x => x.nombre);
                            break;
                        case "nombreJefe":
                            temp = temp.OrderBy(x => x.nombreJefe);
                            break;
                        case "fechaAutoevaluacion":
                            temp = temp.OrderBy(x => x.fechaAutoevaluacion);
                            break;
                        case "fechaEvaluacion":
                            temp = temp.OrderBy(x => x.fechaEvaluacion);
                            break;
                        case "fechaCalibracion":
                            temp = temp.OrderBy(x => x.fechaCalibracion);
                            break;
                        case "fechaFeedback":
                            temp = temp.OrderBy(x => x.fechaFeedback);
                            break;
                        case "idEstado":
                            temp = temp.OrderBy(x => x.idEstado);
                            break;
                    }
                }
                else
                {
                    switch (orden)
                    {
                        case "nombre":
                            temp = temp.OrderByDescending(x => x.nombre);
                            break;
                        case "nombreJefe":
                            temp = temp.OrderByDescending(x => x.nombreJefe);
                            break;
                        case "fechaAutoevaluacion":
                            temp = temp.OrderByDescending(x => x.fechaAutoevaluacion);
                            break;
                        case "fechaEvaluacion":
                            temp = temp.OrderByDescending(x => x.fechaEvaluacion);
                            break;
                        case "fechaCalibracion":
                            temp = temp.OrderByDescending(x => x.fechaCalibracion);
                            break;
                        case "fechaFeedback":
                            temp = temp.OrderByDescending(x => x.fechaFeedback);
                            break;
                        case "idEstado":
                            temp = temp.OrderByDescending(x => x.idEstado);
                            break;
                    }
                }
            }
            else
            {
                temp = temp.OrderByDescending(x => x.ano);
            }
            var lista = temp.ToList();
            return lista;
        }
        public static string normalizarTexto(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }            
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }

        public PerformanceVM buscarPerformance(int? idPerformance)
        {
            using (var db = new PerformanceEntities())
            {
                var tmp = (from d in db.PerformanceColaborador
                           join e in db.Estados on d.estado equals e.id
                           select new PerformanceVM
                           {
                               ano = d.ano,
                               idPerformance = d.idPerformance,
                               idUsuario = d.idUsuario,
                               nombre = d.nombre,
                               idJefe = d.idJefe,
                               nombreJefe = d.nombreJefe,
                               antiguedad = d.antiguedad,
                               fechaAutoevaluacion = d.fechaAutoevaluacion,
                               fechaEvaluacion = d.fechaEvaluacion,
                               fechaCalibracion = d.fechaCalibracion,
                               fechaFeedback = d.fechaEvaluacion, //cambiar
                               idEstado = d.estado,
                               estado = e.estado,
                               habilidades = (from x in db.AutoEvaluacion
                                              where x.idPerformance == d.idPerformance
                                              select new PerformanceAutoevaluacionVM
                                              {
                                                  idPerformance = x.idPerformance,
                                                  idHabilidad = x.idHabilidad,
                                                  idCalificacion = x.idCalificacion
                                              }).Where(x => x.idPerformance == idPerformance).ToList()
                           }).Where(z => z.idPerformance == idPerformance).FirstOrDefault();
                return tmp;
            }
        }

        public void Dispose()
        {
            db.Dispose(); // Liberar recursos del contexto de la base de datos
        }

        public List<PerformanceEstados> listarEstadosPerformance()
        {
            using (var db = new PerformanceEntities())
            {
                var query = (from d in db.Estados
                             select new PerformanceEstados
                             {
                                 id = d.id,
                                 estado = d.estado,
                             });
                return query.ToList();
            }
        }
        public List<PerformanceAnos> listarAnosPerformance()
        {
            List<PerformanceAnos> lista = null;

            lista =
            (from d in db.PerformanceColaborador
             select new PerformanceAnos
             {
                 ano = d.ano,                
             }).Where(x => x.ano != null).Distinct().ToList();

            return lista.OrderBy(x => x.ano).ToList();
        }
        public List<ColaboradorVM> ListarColaboradores()
        {
            List<ColaboradorVM> lista = null;

            lista =
            (from d in db.PerformanceColaborador
             select new ColaboradorVM
             {
                 idUsuario = d.idUsuario,
                 nombre = d.nombre,
             }).Where(x => x.nombre != "").Distinct().ToList();

            return lista.OrderBy(x => x.nombre).ToList();
        }
        public List<ColaboradorVM> ListarLideres()
        {
            List<ColaboradorVM> lista = null;

            lista =
            (from d in db.PerformanceColaborador
             select new ColaboradorVM
             {
                 idUsuario = d.idJefe,
                 nombreJefe = d.nombreJefe,
             }).Where(x => x.nombreJefe != "").Distinct().ToList();

            return lista.OrderBy(x => x.nombreJefe).ToList();
        }
        public List<ColaboradorVM> ListarDominios()
        {
            List<ColaboradorVM> lista = null;

            lista =
            (from d in db.PerformanceColaborador
             where d.dominio != ""
             group d by d.dominio.Substring(0, 3) into g
             select new ColaboradorVM
             {
                 dominio = g.FirstOrDefault().dominio
             }).Distinct().ToList();

            return lista.OrderBy(x => x.convenio).ToList();
        }
        public List<ColaboradorVM> ListarConvenios()
        {
            List<ColaboradorVM> lista = null;

            lista =
            (from d in db.PerformanceColaborador
             where d.convenio != ""
             group d by d.convenio.Substring(0, 3) into g
             select new ColaboradorVM
             {
                 convenio = g.FirstOrDefault().convenio
             }).Distinct().ToList();

            return lista.OrderBy(x => x.convenio).ToList();
        }

        public PerformanceAutoevaluacionVM GuardarAutoevaluacion(PerformanceAutoevaluacionVM autoevaluacion)
        {
            try
            {
                var habilidades = db.Habilidades.Where(x => x.activo == true && x.esEvaluado == true).ToList();

                AutoEvaluacion nuevaAutoevaluacion = new AutoEvaluacion();
                PerformanceAutoevaluacionVM performanceVM = new PerformanceAutoevaluacionVM();

                List<string> calificacion = new List<string>();
                calificacion.Add(autoevaluacion.habilidad1);
                calificacion.Add(autoevaluacion.habilidad2);
                calificacion.Add(autoevaluacion.habilidad3);
                calificacion.Add(autoevaluacion.habilidad4);
                calificacion.Add(autoevaluacion.habilidad5);
                calificacion.Add(autoevaluacion.habilidad6);

                //AutoEvaluacion
                var numero = 1; // Inicializar numero en 0

                foreach (var habilidad in habilidades)
                {
                    if (autoevaluacion != null)
                    {
                        nuevaAutoevaluacion.idPerformance = autoevaluacion.idPerformance;
                        nuevaAutoevaluacion.idHabilidad = habilidad.idHabilidad;
                        nuevaAutoevaluacion.fechaAutoEvaluacion = DateTime.Now;

                        // Obtener la calificación correspondiente a la habilidad actual
                        var habilidadIndex = habilidades.IndexOf(habilidad);
                        var item = calificacion.ElementAtOrDefault(habilidadIndex); // Obtener la calificación para la habilidad actual
                        if (item != null)
                        {
                            var idCalificacion = db.Calificacion
                                .Where(x => x.activo == true && x.formulario == "A" && x.nombre.Contains(item))
                                .Select(x => x.idCalificacion)
                                .FirstOrDefault();

                            if (idCalificacion != 0)
                            {
                                nuevaAutoevaluacion.idCalificacion = idCalificacion;

                                db.AutoEvaluacion.Add(nuevaAutoevaluacion);
                                db.SaveChanges();
                            }
                            else
                            {
                                // Manejar la situación donde no se encuentra una calificación para la habilidad actual
                            }
                        }
                    }
                }


                //Performance
                var performance = db.PerformanceColaborador.Where(x => x.idPerformance == autoevaluacion.idPerformance).FirstOrDefault();

                if (performance != null)
                {
                    performance.estado = 2;
                    performance.fechaAutoevaluacion = DateTime.Now;

                    db.SaveChanges();
                }

                //Envio de mail a lider
                var idDestinatario = performance.idJefe;
                var asunto = "Envío de autoevaluación";
                var body =
                    "<table border=\"0\" width=\"200px\" bgcolor=\"#EDECEB\"> \r\n    " +
                        "<tbody> \r\n        " +
                            "<tr> \r\n            " +
                                "<td align=\"center\"><img src=\"https://buhogestion.distrocuyo.com/content/perform/performanceIdentidad2024.png\" alt=\"\" width=\"150px\"></td> \r\n        " +
                            "</tr> \r\n    " +
                        "</tbody> \r\n" +
                    "</table>\r\n" +
                    "<table style=\"border: 10px solid #edeceb; padding-left: 20px;\" width=\"100%\"> \r\n    " +
                        "<tbody> \r\n        " +
                            "<tr> \r\n            " +
                                "<td><br/><br /><div style=\"color: #353543; font-family: Arial,sans-serif; font-size: 14px; line-height: 22px;\">" +
                                    "<br /> <strong>Estimado/a: #destinatario </strong><br /> \r\n <p>Se informa que #colaborador ha realizado la autoevaluación.<br /><br /><br /><br />\r\n" +
                                "</td>\r\n        " +
                            "</tr>\r\n    " +
                        "</tbody>\r\n" +
                    "</table>\r\n" +
                    "<table style=\"color: #353543; font-family: Arial; font-size: 10px; line-height: 22px; padding-left: 25px;\">\r\n    " +
                        "<tbody>\r\n        " +
                            "<tr>\r\n            " +
                                "<td align=\"right\"><p>Email enviado automaticamente por Buho Gestion </p></td>\r\n        " +
                            "</tr>\r\n    " +
                        "</tbody>\r\n" +
                    "</table>";

                body = body.Replace("#destinatario", performance.nombreJefe);
                body = body.Replace("#colaborador", performance.nombre);

                performanceVM.body = body;
                performanceVM.asunto = asunto;
                performanceVM.idPerformance = performance.idPerformance;

                //HISTORIAL
                Historial historial = new Historial();
                historial.idPerformance = performance.idPerformance;
                historial.estado = 1;
                historial.idUsuarioCambio = autoevaluacion.idUsuario;
                historial.autoevaluacion = true;
                historial.evaluacion = false;
                historial.calibracion = false;
                historial.idHabilidad = null;
                historial.idCalificacion = null;
                historial.idCalificacionFinal = null;
                historial.eliminado = false;
                historial.fechaOriginal = null;
                historial.fechaCambio = DateTime.Now;
                historial.nombreUsuarioCambio = autoevaluacion.nombreUsuario;

                db.Historial.Add(historial);
                db.SaveChanges();

                return performanceVM;
            }
            catch (Exception e)
            {
                var ex = e.GetBaseException();
                PerformanceAutoevaluacionVM performanceVM = new PerformanceAutoevaluacionVM();
                performanceVM.idPerformance = 0;
                return performanceVM;
            }
        }

        public PerformanceColaborador EliminarPerformance(int idPerformance, int idUsuario, string nombreUsuario)
        {
            //Cambia eliminado a true

            var performance = db.PerformanceColaborador.Where(x => x.idPerformance == idPerformance).FirstOrDefault();

            performance.eliminado = true;

            db.SaveChanges();

            //Guarda movimiento en el historial
            Historial historial = new Historial();
            historial.idPerformance = performance.idPerformance;
            historial.estado = performance.estado;
            historial.idUsuarioCambio = idUsuario;
            historial.autoevaluacion = false;
            historial.evaluacion = false;
            historial.calibracion = false;
            historial.idHabilidad = null;
            historial.idCalificacion = null;
            historial.idCalificacionFinal = null;
            historial.eliminado = true;
            historial.fechaOriginal = null;
            historial.fechaCambio = DateTime.Now;
            historial.nombreUsuarioCambio = nombreUsuario;

            db.Historial.Add(historial);
            db.SaveChanges();

            return performance;

        }
        public PerformanceAutoevaluacionVM GuardarEvaluacion(PerformanceAutoevaluacionVM evaluacion, int idResponsable)
        {
            try
            {
                var evaluacionesExistentes = db.EvaluacionPerformance.Where(x => x.idPerformance == evaluacion.idPerformance).ToList();

                if (!evaluacionesExistentes.Any())
                {                    
                    var habilidades = db.Habilidades.Where(x => x.activo == true && x.esEvaluado == true).ToList();

                    EvaluacionPerformance nuevaEvaluacion = new EvaluacionPerformance();
                    PerformanceAutoevaluacionVM performanceVM = new PerformanceAutoevaluacionVM();

                    List<string> calificacion = new List<string>();
                    calificacion.Add(evaluacion.habilidad1);
                    calificacion.Add(evaluacion.habilidad2);
                    calificacion.Add(evaluacion.habilidad3);
                    calificacion.Add(evaluacion.habilidad4);
                    calificacion.Add(evaluacion.habilidad5);
                    calificacion.Add(evaluacion.habilidad6);

                    //Evaluacion
                    var numero = 1; // Inicializar numero en 0

                    foreach (var habilidad in habilidades)
                    {
                        if (evaluacion != null)
                        {
                            nuevaEvaluacion.idPerformance = evaluacion.idPerformance;
                            nuevaEvaluacion.idHabilidad = habilidad.idHabilidad;
                            nuevaEvaluacion.fechaEvaluacion = DateTime.Now;
                            nuevaEvaluacion.idResponsable = evaluacion.idResponsable;

                            // Obtener la calificación correspondiente a la habilidad actual
                            var habilidadIndex = habilidades.IndexOf(habilidad);
                            var item = calificacion.ElementAtOrDefault(habilidadIndex); // Obtener la calificación para la habilidad actual
                            if (item != null)
                            {
                                var idCalificacion = db.Calificacion
                                    .Where(x => x.activo == true && x.formulario == "A" && x.nombre.Contains(item))
                                    .Select(x => x.idCalificacion)
                                    .FirstOrDefault();

                                if (idCalificacion != 0)
                                {
                                    nuevaEvaluacion.idCalificacion = idCalificacion;

                                    db.EvaluacionPerformance.Add(nuevaEvaluacion);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    // Manejar la situación donde no se encuentra una calificación para la habilidad actual
                                }
                            }
                        }
                    }


                    //Performance
                    var performance = db.PerformanceColaborador.Where(x => x.idPerformance == evaluacion.idPerformance).FirstOrDefault();

                    if (performance != null)
                    {
                        performance.estado = 3;
                        performance.calificacionFinal = evaluacion.calificacionFinal;
                        performance.fechaEvaluacion = DateTime.Now;
                        var idCalificacionFinal = db.CalificacionFinalLider
                                   .Where(x => x.nombre.Contains(evaluacion.calificacionFinal))
                                   .Select(x => x.id)
                                   .FirstOrDefault();
                        performance.idCalificacionFinal = idCalificacionFinal;

                        db.SaveChanges();
                    }

                    //HISTORIAL
                    Historial historial = new Historial();
                    historial.idPerformance = performance.idPerformance;
                    historial.estado = 2;
                    historial.nombreUsuarioCambio = evaluacion.nombreUsuario;
                    historial.idUsuarioCambio = idResponsable;
                    historial.autoevaluacion = false;
                    historial.evaluacion = true;
                    historial.calibracion = false;
                    historial.idHabilidad = null;
                    historial.idCalificacion = null;
                    historial.idCalificacionFinal = null;
                    historial.eliminado = false;
                    historial.fechaOriginal = null;
                    historial.fechaCambio = DateTime.Now;

                    db.Historial.Add(historial);
                    db.SaveChanges();

                    return evaluacion;
                }
                else
                {
                    List<string> calificaciones = new List<string>
                     {
                        evaluacion.habilidad1,
                        evaluacion.habilidad2,
                        evaluacion.habilidad3,
                        evaluacion.habilidad4,
                        evaluacion.habilidad5,
                        evaluacion.habilidad6
                    };

                    foreach (var evaluacionExistente in evaluacionesExistentes)
                    {
                        var habilidad = db.Habilidades.Where(x => x.idHabilidad == evaluacionExistente.idHabilidad).FirstOrDefault();
                        if (habilidad != null)
                        {
                            int habilidadIndex = evaluacionesExistentes.IndexOf(evaluacionExistente);
                            string calificacion = calificaciones.ElementAtOrDefault(habilidadIndex);

                            if (calificacion != null)
                            {
                                var idCalificacion = db.Calificacion
                                    .Where(x => x.activo == true && x.formulario == "A" && x.nombre.Contains(calificacion))
                                    .Select(x => x.idCalificacion)
                                    .FirstOrDefault();

                                if (idCalificacion != 0)
                                {
                                    evaluacionExistente.idCalificacion = idCalificacion;
                                    evaluacionExistente.fechaEvaluacion = DateTime.Now;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }

                    // Actualizar el estado y la calificación final de Performance
                    var performance = db.PerformanceColaborador.Where(x => x.idPerformance == evaluacion.idPerformance).FirstOrDefault();
                    if (performance != null)
                    {
                        performance.estado = 3;
                        performance.calificacionFinal = evaluacion.calificacionFinal;
                        performance.fechaEvaluacion = DateTime.Now;

                        var idCalificacionFinal = db.CalificacionFinalLider
                            .Where(x => x.nombre.Contains(evaluacion.calificacionFinal))
                            .Select(x => x.id)
                            .FirstOrDefault();

                        performance.idCalificacionFinal = idCalificacionFinal;
                        db.SaveChanges();
                    }

                    //HISTORIAL
                    Historial historial = new Historial();
                    historial.idPerformance = performance.idPerformance;
                    historial.estado = performance.estado;
                    historial.nombreUsuarioCambio = evaluacion.nombreUsuario;
                    historial.idUsuarioCambio = idResponsable;
                    historial.autoevaluacion = false;
                    historial.evaluacion = true;
                    historial.calibracion = false;
                    historial.idHabilidad = null;
                    historial.idCalificacion = null;
                    historial.idCalificacionFinal = null;
                    historial.eliminado = false;
                    historial.fechaOriginal = null;
                    historial.fechaCambio = DateTime.Now;

                    db.Historial.Add(historial);
                    db.SaveChanges();

                    return evaluacion;
                }                
            }
            catch (Exception e)
            {
                var ex = e.GetBaseException();
                PerformanceAutoevaluacionVM performanceVM = new PerformanceAutoevaluacionVM();
                performanceVM.idPerformance = 0;
                return performanceVM;
            }
        }
        public PerformanceCalibracionVM GuardarCalibracion(PerformanceCalibracionVM calibracion)
        {
            try
            {
                var evaluacionesExistentes = db.EvaluacionPerformance.Where(x => x.idPerformance == calibracion.idPerformance).ToList();
                var performance = db.PerformanceColaborador.Where(x => x.idPerformance == calibracion.idPerformance).FirstOrDefault();
                var calificacionFinal = db.CalificacionFinalLider.Where(x => x.nombre == calibracion.calificacionFinal).FirstOrDefault();
                var calificacionAntes = db.CalificacionFinalLider.Where(x => x.id == performance.idCalificacionFinal).FirstOrDefault();

                performance.estado = 4;
                
                if (performance.calificacionFinal != null)
                {
                    performance.calificacionFinalAntes = calificacionAntes.nombre;
                    performance.idCalificacionFinalAntes = calificacionAntes.id;
                }
                else
                {
                    performance.calificacionFinalAntes = "No calificado por el líder";
                }
                performance.idCalificacionFinal = calificacionFinal.id;
                performance.calificacionFinal = calificacionFinal.nombre;
                performance.fechaCalibracion = DateTime.Now;
                performance.comentario = calibracion.comentario;

                db.SaveChanges();

                Historial nuevaEvaluacion = new Historial();

                nuevaEvaluacion.idPerformance = performance.idPerformance;
                nuevaEvaluacion.idUsuarioCambio = calibracion.idResponsable;
                nuevaEvaluacion.nombreUsuarioCambio = calibracion.nombreUsuario;
                nuevaEvaluacion.idHabilidad = null;
                nuevaEvaluacion.idCalificacion = null;
                nuevaEvaluacion.idCalificacionFinal = calificacionFinal.id;
                if(calificacionAntes != null)
                {
                    nuevaEvaluacion.idCalificacionFinalAntes = calificacionAntes.id;
                }               
                nuevaEvaluacion.fechaOriginal = performance.fechaEvaluacion;
                nuevaEvaluacion.idResponsableOriginal = performance.idJefe;
                nuevaEvaluacion.calibracion = true;
                nuevaEvaluacion.fechaCambio = DateTime.Now;
                nuevaEvaluacion.estado = 3;
                db.Historial.Add(nuevaEvaluacion);
                db.SaveChanges();

                
                return calibracion;
            }
            catch (Exception e)
            {
                var ex = e.GetBaseException();
                PerformanceCalibracionVM performanceVM = new PerformanceCalibracionVM();
                performanceVM.idPerformance = 0;
                return performanceVM;
            }
        }
        public int GenerarAltasPerformance(List<ColaboradorVM> colaboradores)
        {
            try
            {      
                int anoActual = DateTime.Now.Year;

                PerformanceColaborador nuevaPerformance = new PerformanceColaborador();

                foreach (var item in colaboradores)
                {
                    var existe = db.PerformanceColaborador.Where(x => x.ano == anoActual && x.idUsuario == item.idUsuario).FirstOrDefault();
                    if(existe == null)
                    {
                        nuevaPerformance.ano = anoActual;
                        nuevaPerformance.idUsuario = item.idUsuario;
                        nuevaPerformance.legajo = item.legajo;
                        nuevaPerformance.nombre = item.nombre;
                        nuevaPerformance.sexo = item.sexo;
                        nuevaPerformance.pais = item.pais;
                        nuevaPerformance.convenio = item.convenio;
                        nuevaPerformance.categoria = item.categoria;
                        nuevaPerformance.dominio = item.dominio;                       
                        nuevaPerformance.idJefe = item.idJefe;
                        nuevaPerformance.nombreJefe = item.nombreJefe;
                        nuevaPerformance.estado = 1;
                        nuevaPerformance.edad = 0;
                        nuevaPerformance.antiguedad = 0;
                        //calculo edad
                        if (item.fechaNacimiento != null)
                        {
                            DateTime fechaNacimiento = item.fechaNacimiento.Value;
                            DateTime fechaActual = DateTime.Today;
                            int edad = fechaActual.Year - fechaNacimiento.Year;
                            // Verifica si el cumpleaños de este año ya ha pasado
                            if (fechaActual < fechaNacimiento.AddYears(edad))
                            {
                                edad--;
                            }
                            nuevaPerformance.edad = edad;
                        }
                        
                      if(item.fechaIngreso != null)
                        {
                            //calculo antiguedad
                            int anoIngreso = item.fechaIngreso.Value.Year;
                            int antiguedad = Math.Abs(anoActual - anoIngreso);
                            nuevaPerformance.antiguedad = antiguedad;
                        }                      
                        
                        db.PerformanceColaborador.Add(nuevaPerformance);
                        db.SaveChanges();
                    }                  
                }

                return anoActual;
            }
            catch (Exception e)
            {
                var ex = e.GetBaseException();
                return 0;
            }
        }
        /// <summary>
        /// calulos progreso de estados 
        /// </summary>
        /// <returns></returns>
        public List<PerformanceProgresoVM> listarPerformanceProgreso()
        {
            var list = ListarPerformanceTodas();
            var anoAnterior = DateTime.Now.Year;
            var tmp = list.Where(x => x.ano == anoAnterior).ToList();
            var totalPerformance = 0;
            var totalPendiente= 0;
            var totalCompletado = 0;
            var totalEvaluado = 0;
            var totalFeedback = 0;
            var totalFinalizado = 0;
            decimal porcentajePendiente = 0;
            decimal porcentajeCompletado = 0;
            decimal porcentajeEvaluado = 0;
            decimal porcentajeFeedback = 0;
            decimal porcentajeFinalizado = 0;

            foreach (var item in tmp)
            {
                totalPerformance++;
                if (item.idEstado == 1)
                    totalPendiente++;
                else if (item.idEstado == 2)
                    totalCompletado++;
                else if (item.idEstado == 3)
                    totalEvaluado++;
                else if (item.idEstado == 4)
                    totalFeedback++;
                else if (item.idEstado == 5)
                    totalFinalizado++;
            }
            
            if (totalPerformance > 0)
            {
                porcentajePendiente = (decimal)totalPendiente / totalPerformance * 100;
                porcentajeCompletado = (decimal)totalCompletado / totalPerformance * 100;
                porcentajeEvaluado = (decimal)totalEvaluado / totalPerformance * 100;
                porcentajeFeedback = (decimal)totalFeedback / totalPerformance * 100;
                porcentajeFinalizado = (decimal)totalFinalizado / totalPerformance * 100;
            }

            var lista = new List<PerformanceProgresoVM>();
           
            lista.Add(new PerformanceProgresoVM
            {                
                totalPerformance = totalPerformance,
                totalPendiente = totalPendiente,
                totalCompletado = totalCompletado,
                totalEvaluado = totalEvaluado,
                totalFeedback = totalFeedback,
                totalFinalizado = totalFinalizado,
                porcentajePendiente = porcentajePendiente,
                porcentajeCompletado = porcentajeCompletado,
                porcentajeEvaluado = porcentajeEvaluado,
                porcentajeFeedback = porcentajeFeedback,
                porcentajeFinalizado = porcentajeFinalizado
            });
            
            return lista;
        }
        public DatosGeneralPerformanceVM buscarDatosPerformance(int idPerformance, int perfil)
        {
            DatosGeneralPerformanceVM objeto = new DatosGeneralPerformanceVM();
            var datosPerformance = (from p in db.PerformanceColaborador
                         where p.idPerformance == idPerformance
                         select new DatosPerformanceVM
                         {
                             ano = p.ano,
                             colaborador = p.nombre,
                             lider = p.nombreJefe,
                             idCalificacionFinal = p.idCalificacionFinal,
                             calificacionFinal = p.calificacionFinal,
                             nombreJefe = p.nombreJefe,
                             comentario = perfil != 127 ? p.comentario : null,
                         }).ToList();

            var autoEvaluaciones = (from a in db.AutoEvaluacion
                                    join ha in db.Habilidades on a.idHabilidad equals ha.idHabilidad into habilidadesAutoEvaluacion
                                    from ha in habilidadesAutoEvaluacion.DefaultIfEmpty()
                                    join ce in db.Calificacion on a.idCalificacion equals ce.idCalificacion into calificacionesAutoEvaluacion
                                    from ca in calificacionesAutoEvaluacion.DefaultIfEmpty()
                                    where a.idPerformance == idPerformance
                                    select new DatosPerformanceVM
                                    {
                                        idHabilidadAutoevaluacion = ha.idHabilidad,
                                        idCalificacionAutoevaluacion = ca.idCalificacion,
                                        fechaCalificacionAutoevaluacion = a.fechaAutoEvaluacion,
                                        nombreHabilidadAutoevaluacion = ha.habilidad,
                                        calificacionAutoevaluacion = ca.nombre,
                                        idHabilidadEvaluacion = null,
                                        idCalificacionEvaluacion = null,
                                        fechaCalificacionEvaluacion = null,
                                        nombreHabilidadEvaluacion = null,
                                        calificacionEvaluacion = null,                                       
                                    }).ToList();
            var evaluaciones = (
                from a in db.EvaluacionPerformance
                join he in db.Habilidades on a.idHabilidad equals he.idHabilidad into habilidadesEvaluacion
                from he in habilidadesEvaluacion.DefaultIfEmpty()
                join ce in db.Calificacion on a.idCalificacion equals ce.idCalificacion into calificacionesEvaluacion
                from ce in calificacionesEvaluacion.DefaultIfEmpty()
                where a.idPerformance == idPerformance
                select new DatosPerformanceVM
                {
                    idHabilidadAutoevaluacion = null,
                    idCalificacionAutoevaluacion = null,
                    fechaCalificacionAutoevaluacion = null,
                    nombreHabilidadAutoevaluacion = null,
                    calificacionAutoevaluacion = null,
                    idHabilidadEvaluacion = he.idHabilidad,
                    idCalificacionEvaluacion = ce.idCalificacion,
                    fechaCalificacionEvaluacion = a.fechaEvaluacion,
                    nombreHabilidadEvaluacion = he.habilidad,
                    calificacionEvaluacion = ce.nombre,                   

                }).ToList();

            datosPerformance.AddRange(autoEvaluaciones);
            datosPerformance.AddRange(evaluaciones);

            objeto.listDatosPerformance = datosPerformance;
            var performance = db.PerformanceColaborador.Where(x => x.idPerformance == idPerformance).FirstOrDefault();
            objeto.idCalificacionFinal = performance?.idCalificacionFinal ?? null;

            return objeto;
        }

        public ReporteExcelVM GenerarExcelUnColaborador(int idPerformance)
        {
            using (PerformanceEntities _db = new PerformanceEntities())
            {
                var tmp = (from a in _db.AutoEvaluacion
                           join p in _db.PerformanceColaborador on a.idPerformance equals p.idPerformance
                           join e in _db.Estados on p.estado equals e.id
                           join c in _db.Calificacion on a.idCalificacion equals c.idCalificacion
                           join h in _db.Habilidades on a.idHabilidad equals h.idHabilidad
                           where a.idPerformance == idPerformance
                           select new DatosPerformanceVM
                           {
                               idAutoevaluacion = a.idAutoEvaluacion,
                               ano = p.ano,
                               idPerformance = a.idPerformance,
                               idUsuario = p.idUsuario,
                               legajo = p.legajo,
                               edad = p.edad,
                               sexo = p.sexo,
                               pais = p.pais,
                               convenio = p.convenio,
                               dominio = p.dominio,
                               categoria = p.categoria,
                               colaborador = p.nombre,
                               idJefe = p.idJefe,
                               nombreJefe = p.nombreJefe,
                               antiguedad = p.antiguedad,
                               fechaCalificacionAutoevaluacion = p.fechaAutoevaluacion,
                               fechaCalificacionEvaluacion = p.fechaEvaluacion,
                               fechaCalibracion = p.fechaCalibracion,
                               fechaFeedback = p.fechaEvaluacion, //cambiar
                               idEstado = p.estado,
                               estado = e.estado,
                               idHabilidad = a.idHabilidad,
                               idCalificacion = a.idCalificacion,
                               calificacion = c.nombre,
                               habilidad = h.habilidad,
                               calificacionFinal = p.calificacionFinal
                           }).OrderBy(x => x.idHabilidad);
                var list = tmp.ToList();

                ReporteExcelVM excel = new ReporteExcelVM();
                excel.filas = new List<DetalleExcelVM>();
                excel.encabezado = new List<string>();
                DetalleExcelVM detalle = new DetalleExcelVM();
                excel.filas.Add(detalle);

                ExcelUtility excelUtility = new ExcelUtility();
                excel = excelUtility.GenerarUnReportePerformance(list);
                return excel;
            }

        }
        public ReporteExcelVM GenerarExcelReporteColaboradores()
        {
            using (PerformanceEntities _db = new PerformanceEntities())
            {
                var tmp = (from p in _db.PerformanceColaborador
                           join e in _db.Estados on p.estado equals e.id
                           select new DatosPerformanceVM
                           {
                               ano = p.ano,
                               idPerformance = p.idPerformance,
                               idUsuario = p.idUsuario,
                               colaborador = p.nombre,
                               idJefe = p.idJefe,
                               nombreJefe = p.nombreJefe,
                               antiguedad = p.antiguedad,
                               fechaCalificacionAutoevaluacion = p.fechaAutoevaluacion,
                               fechaCalificacionEvaluacion = p.fechaEvaluacion,
                               fechaCalibracion = p.fechaCalibracion,
                               fechaFeedback = null, //cambiar
                               idEstado = p.estado,
                               estado = e.estado,
                               calificacionFinal = p.calificacionFinal,
                           }).OrderByDescending(x => x.ano).ThenBy(x => x.colaborador);
                var list = tmp.ToList();

                ReporteExcelVM excel = new ReporteExcelVM();
                excel.filas = new List<DetalleExcelVM>();
                excel.encabezado = new List<string>();
                DetalleExcelVM detalle = new DetalleExcelVM();
                excel.filas.Add(detalle);

                ExcelUtility excelUtility = new ExcelUtility();
                excel = excelUtility.GenerarReporteColaboradores(list);
                return excel;
            }

        }
        public ReporteExcelVM GenerarExcelReportesColaboradores(int? colaborador, int? estado, int? lider, int? ano)
        {
            using (PerformanceEntities _db = new PerformanceEntities())
            {
                var query = (from p in _db.PerformanceColaborador
                           join e in _db.Estados on p.estado equals e.id
                           select new
                           {
                               p.ano,
                               p.idPerformance,
                               p.idUsuario,
                               p.legajo,
                               p.nombre,
                               p.edad,
                               p.sexo,
                               p.pais,
                               p.convenio,
                               p.categoria,
                               p.dominio,
                               p.idJefe,
                               p.nombreJefe,
                               p.antiguedad,
                               p.fechaAutoevaluacion,
                               p.fechaEvaluacion,
                               p.fechaCalibracion,
                               p.estado,
                               p.calificacionFinal,
                               p.calificacionFinalAntes,
                               estadoNombre = e.estado,
                               autoEvaluaciones = _db.AutoEvaluacion
                                                   .Where(a => a.idPerformance == p.idPerformance)
                                                   .Select(a => (int?)a.idCalificacion)
                                                   .Take(6)
                                                   .ToList(),
                               evaluaciones = _db.EvaluacionPerformance
                                                 .Where(ev => ev.idPerformance == p.idPerformance)
                                                 .Select(ev => (int?)ev.idCalificacion)
                                                 .Take(6)
                                                 .ToList()
                           });
                // Aplicar los filtros según los parámetros proporcionados
                if (colaborador.HasValue)
                {
                    query = query.Where(p => p.idUsuario == colaborador);
                }
                if (estado.HasValue)
                {
                    query = query.Where(p => p.estado == estado);
                }
                if (lider.HasValue)
                {
                    query = query.Where(p => p.idJefe == lider);
                }
                if (ano.HasValue)
                {
                    query = query.Where(p => p.ano == ano);
                }
                var tmp = query.OrderByDescending(x => x.ano).ThenBy(x => x.nombre);
                var list = tmp.ToList().Select(x => new DatosPerformanceVM
                {
                    ano = x.ano,
                    idPerformance = x.idPerformance,
                    idUsuario = x.idUsuario,
                    legajo = x.legajo,
                    colaborador = x.nombre,
                    edad = x.edad,
                    sexo = x.sexo,
                    pais = x.pais,
                    convenio = x.convenio,
                    categoria = x.categoria,
                    dominio = x.dominio,
                    idJefe = x.idJefe,
                    nombreJefe = x.nombreJefe,
                    antiguedad = x.antiguedad,
                    fechaCalificacionAutoevaluacion = x.fechaAutoevaluacion,
                    fechaCalificacionEvaluacion = x.fechaEvaluacion,
                    fechaCalibracion = x.fechaCalibracion,
                    fechaFeedback = null, // cambiar
                    idEstado = x.estado,
                    estado = x.estadoNombre,
                    autoEvaluaciones = x.autoEvaluaciones,
                    evaluaciones = x.evaluaciones,
                    calificacionFinal = x.calificacionFinal,
                    calificacionFinalAntes = x.calificacionFinalAntes,
                }).ToList();

                ReporteExcelVM excel = new ReporteExcelVM();
                excel.filas = new List<DetalleExcelVM>();
                excel.encabezado = new List<string>();
                DetalleExcelVM detalle = new DetalleExcelVM();
                excel.filas.Add(detalle);

                ExcelUtility excelUtility = new ExcelUtility();
                excel = excelUtility.GenerarReporteColaboradores(list);
                return excel;
            }
        }
        public void CambiarEstado(int idPerformance, int estado, int idUsuario, string nombreUsuario)
        {
            var performance = db.PerformanceColaborador.Where(x => x.idPerformance == idPerformance).FirstOrDefault();
            Historial historial = new Historial();

            performance.estado = estado;
            db.SaveChanges();

            historial.idPerformance = idPerformance;
            historial.estado = 6;
            historial.idUsuarioCambio = idUsuario;
            historial.fechaCambio = DateTime.Now;
            historial.nombreUsuarioCambio = nombreUsuario;
            db.Historial.Add(historial);
            db.SaveChanges();
        }
        public int EstadoActual()
        {
            int anoActual = DateTime.Now.Year;
            DateTime fechaActual = DateTime.Now.Date; // Obtiene solo la parte de la fecha
            int estadoActual = 0;

            var estadosFechas = db.EstadosFechas
                                  .Where(x => x.ano == anoActual)
                                  .ToList();

            foreach (var estadoFecha in estadosFechas)
            {
                DateTime fechaDesde = estadoFecha.fechaDesde?.Date ?? DateTime.MinValue;
                DateTime fechaHasta = estadoFecha.fechaHasta?.Date ?? DateTime.MaxValue;

                if (fechaActual >= fechaDesde && fechaActual <= fechaHasta)
                {
                    estadoActual = estadoFecha.estado;
                    break; // Salir del bucle si se encuentra el estado actual
                }
            }
            return estadoActual;
        }

        public List<HistorialVM> ListarHistorial(int idPerformance, int idUsuario)
        {
            var tmp = (from h in db.Historial
                       join p in db.PerformanceColaborador on h.idPerformance equals p.idPerformance
                       join e in db.Estados on h.estado equals e.id
                       where h.idPerformance == idPerformance
                       select new HistorialVM
                       {
                           idHistorial = h.idHistorial,
                           idPerformance = h.idPerformance,
                           estado = h.estado,
                           idUsuarioCambio = h.idUsuarioCambio,
                           idHabilidad = h.idHabilidad,
                           idCalificacion = h.idCalificacion,
                           idCalificacionFinal = h.idCalificacionFinal,
                           fechaOriginal = h.fechaOriginal,
                           eliminado = h.eliminado,
                           autoevaluacion = h.autoevaluacion,
                           evaluacion = h.evaluacion,
                           calibracion = h.calibracion,
                           fechaCambio = h.fechaCambio,
                           estadoStr = e.estado,
                           nombreUsuario = h.nombreUsuarioCambio ?? "",
                       }).OrderBy(x => x.idHistorial).ToList();

            foreach(var item in tmp)
            {
                if (item.autoevaluacion == true)
                {
                    item.estadoStr = "Autoevaluación";
                }
                else if (item.evaluacion == true)
                {
                    item.estadoStr = "Evaluación";
                }
                else if (item.calibracion == true)
                {
                    item.estadoStr = "Calibración";
                }
                else if (item.estado == 6)
                {
                    item.estadoStr = "Regreso a lider";
                }
            }
            return tmp;
        }
        /// <summary>
        /// PDI
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="idPerformance"></param>
        /// <returns></returns>
        public List<PDIMetasVM> buscarDatosPDI(int idUsuario, int idPerformance)
        {           
            //si no envia idPerformance buscar idUsuario logueado
            var idUsuarioPDI = idUsuario;
            if (idPerformance != 0)
            {
                var performance = db.PerformanceColaborador.Where(x => x.idPerformance == idPerformance).FirstOrDefault();
                idUsuarioPDI = performance.idUsuario;
            }

            //si no tiene PDI agregar uno     
            var plan = db.PDIColaborador.Where(x => x.idUsuario == idUsuarioPDI).FirstOrDefault();
            var idPdi = 0;
            if (plan != null)
            {
                idPdi = plan.idPDI;
            }
            else
            {
                PDIColaborador nuevoPDI = new PDIColaborador();
                nuevoPDI.idUsuario = idUsuarioPDI;
                db.PDIColaborador.Add(nuevoPDI);
                db.SaveChanges();
                var pdiNuevo = db.PDIColaborador.Where(x => x.idUsuario == idUsuarioPDI).FirstOrDefault();
                idPdi = pdiNuevo.idPDI;
            }               
           
            var datosPDI = (from p in db.PerformanceColaborador
                            join pf in db.PDIColaborador on p.idUsuario equals pf.idUsuario
                            where p.idUsuario == idUsuarioPDI
                            select new PDIMetasVM
                            {
                                idUsuario = p.idUsuario,
                                colaborador = p.nombre,
                                lider = p.nombreJefe,
                                dominio = p.dominio,

                            }).ToList();
            var acciones = (from p in db.PDIColaboradorMetas
                            join t in db.PDItipoAccion on p.tipoAccion equals t.idTipoAccion into tipoAccion
                            from ta in tipoAccion.DefaultIfEmpty()
                            join h in db.Habilidades on p.habilidad equals h.idHabilidad into habilidades
                            from ha in habilidades.DefaultIfEmpty()
                            join s in db.PDIStatus on p.status equals s.idStatus into status
                            from st in status.DefaultIfEmpty()
                            where p.idPDI == idPdi
                            select new PDIMetasVM
                            {
                                idPDI = p.idPDI,
                                idMeta = p.idMeta,
                                nombreTipoAccion = ta.nombre,
                                tipoAccion = ta.idTipoAccion,
                                nombreHabilidad = ha.habilidad,
                                metaTitulo = p.metaTitulo,
                                metaDescripcion = p.metaDescripcion,
                                fechaDesde = p.fechaDesde,
                                fechaHasta = p.fechaHasta,
                                nombreStatus = st.nombre,
                                accionesRealizadas = p.accionesRealizadas,
                            }).ToList();

            datosPDI.AddRange(acciones);
            return datosPDI;
        }
      
        public List<TipoAccionVM> ListarTipoAccion()
        {
            List<TipoAccionVM> lista = null;

            lista =
            (from t in db.PDItipoAccion
             select new TipoAccionVM
             {
                 idTipoAccion = t.idTipoAccion,
                 nombre = t.nombre,
             }).ToList();

            return lista.OrderBy(x => x.idTipoAccion).ToList();
        }
        public List<StatusVM> ListarStatus()
        {
            List<StatusVM> lista = null;

            lista =
            (from a in db.PDIStatus
             select new StatusVM
             {
                 idStatus= a.idStatus,
                 nombre = a.nombre,
             }).ToList();

            return lista.OrderBy(x => x.idStatus).ToList();
        }
        public List<HabilidadesVM> ListarHabilidades()
        {
            List<HabilidadesVM> lista = null;

            lista =
            (from h in db.Habilidades
             select new HabilidadesVM
             {
                 idHabilidad = h.idHabilidad,
                 habilidad = h.habilidad,
             }).ToList();

            return lista.OrderBy(x => x.idHabilidad).ToList();
        }
        public int GuardarMeta(PDIMetasVM meta)
        {
            try
            {     
                if(meta.idUsuario != null)
                {                  

                var pdiColaborador = db.PDIColaborador.Where(x => x.idUsuario == meta.idUsuario).FirstOrDefault();

                PDIColaboradorMetas nuevaMeta = new PDIColaboradorMetas();
                    nuevaMeta.idPDI = pdiColaborador.idPDI;
                    nuevaMeta.tipoAccion = meta.tipoAccion;
                    nuevaMeta.habilidad = meta.habilidad;
                    nuevaMeta.metaTitulo = meta.metaTitulo;
                    nuevaMeta.metaDescripcion = meta.metaDescripcion;
                    nuevaMeta.fechaDesde = meta.fechaDesde;
                    nuevaMeta.fechaHasta = meta.fechaHasta;
                    nuevaMeta.status = meta.status;
                    nuevaMeta.accionesRealizadas = meta.accionesRealizadas;

                db.PDIColaboradorMetas.Add(nuevaMeta);
                db.SaveChanges();
                }
                return 0;
                
            }
            catch (Exception e)
            {
                var ex = e.GetBaseException();
               
                return 1;
            }
        }
        public PDIMetasVM buscarDatosMeta(int idUsuario, int idMeta)
        {
            PDIMetasVM objeto = new PDIMetasVM();
            var datos = (from p in db.PDIColaboradorMetas
                            join pf in db.PDIColaborador on p.idPDI equals pf.idPDI
                            join c in db.PerformanceColaborador on pf.idUsuario equals c.idUsuario
                            join t in db.PDItipoAccion on p.tipoAccion equals t.idTipoAccion into tipoAccion
                            from ta in tipoAccion.DefaultIfEmpty()
                            join h in db.Habilidades on p.habilidad equals h.idHabilidad into habilidades
                            from ha in habilidades.DefaultIfEmpty()
                            join s in db.PDIStatus on p.status equals s.idStatus into status
                            from st in status.DefaultIfEmpty()
                            where p.idMeta == idMeta
                            select new PDIMetasVM
                            {
                                idUsuario = c.idUsuario,
                                colaborador = c.nombre,
                                lider = c.nombreJefe,
                                dominio = c.dominio,                             
                                idPDI = p.idPDI,
                                idMeta = p.idMeta,
                                nombreTipoAccion = ta.nombre,
                                tipoAccion = ta.idTipoAccion,
                                nombreHabilidad = ha.habilidad,
                                metaTitulo = p.metaTitulo,
                                metaDescripcion = p.metaTitulo,
                                fechaDesde = p.fechaDesde,
                                fechaHasta = p.fechaHasta,
                                nombreStatus = st.nombre,
                                accionesRealizadas = p.accionesRealizadas,

                            }).FirstOrDefault();   

            return datos;
        }
        public void cambiarFeedback(int idUsuario, string nombreUsuario)
        {
            int anoActual = DateTime.Now.Year;
            var performance = db.PerformanceColaborador.Where(x => x.ano == anoActual) .ToList();
            Historial historial = new Historial();

            foreach (var item  in performance)
            {
                item.estado = 7;
                db.SaveChanges();

                //historial guardar si no ha cambiado previamente a feedback
                var existe = db.Historial.Where(x => x.idPerformance == item.idPerformance && x.estado == 7).FirstOrDefault();
                if (existe == null)
                {
                    historial.idPerformance = item.idPerformance;
                    historial.estado = 7;
                    historial.idUsuarioCambio = idUsuario;
                    historial.fechaCambio = DateTime.Now;
                    historial.nombreUsuarioCambio = nombreUsuario;
                    db.Historial.Add(historial);
                    db.SaveChanges();
                }               
            }   
        }
    }
}
