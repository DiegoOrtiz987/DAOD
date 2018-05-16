using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Plugin.ValidarStockBibliotecaLibro
{
    public class ValidarStockBibliotecaLibro : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public ValidarStockBibliotecaLibro(string unsecureConfig, string secureConfig)
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecureConfig;
        }
        #endregion
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            Entity entity = (Entity)context.InputParameters["Target"];

            if (entity.LogicalName.Equals("dao_bibliotecalibro"))
                {

                var evento = context.MessageName.ToLower();
                Int16 unidadDigitda = Int16.Parse(entity.Attributes["dao_unidades"].ToString());


                if (evento.Equals("create"))
                {
                    Guid idBibliotecaEntidad = ((EntityReference)entity.Attributes["dao_bibliotecaid"]).Id;
                    Guid idLibroEntidad = ((EntityReference)entity.Attributes["dao_libroid"]).Id;

                    if (unidadDigitda >= 10 || unidadDigitda <= 0)
                    {
                        throw new InvalidPluginExecutionException("*** La cantidad a ingresar debe estar entre 1 y 9 incluyentes ***");
                    }

                    String consultaRelacion = @"<fetch top='1' no-lock='true' >
                                                          <entity name='dao_bibliotecalibro' >
                                                            <attribute name='dao_bibliotecaid' />
                                                            <attribute name='dao_libroid' />
                                                            <filter type='and' >
                                                              <condition attribute='dao_bibliotecaid' operator='eq' value=' " + idBibliotecaEntidad + @"' />
                                                              <condition attribute='dao_libroid' operator='eq' value='" + idLibroEntidad + @"' />
                                                            </filter>
                                                          </entity>
                                                        </fetch>";

                    FetchExpression existeRelacion = new FetchExpression(consultaRelacion);
                    var valorConsultaRelacion = service.RetrieveMultiple(existeRelacion);

                    if ((int.Parse(valorConsultaRelacion.Entities.Count.ToString()) == 1))
                    {
                        throw new InvalidPluginExecutionException("*** Esta relación (Biblioteca-Libro) ya se encuentra registrada ***");
                    }
                }else if (evento.Equals("update"))
                {
                    if (unidadDigitda >= 10 || unidadDigitda <= 0)
                    {
                        throw new InvalidPluginExecutionException("*** La cantidad a ingresar debe estar entre 1 y 9 incluyentes ***");
                    }
                }
            }
        
        }
    }
}