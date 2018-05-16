using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Plugin.ValidarNombreBiblioteca
{
    public class ValidarNombreBiblioteca : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public ValidarNombreBiblioteca(string unsecureConfig, string secureConfig)
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

            //Identificar el evento ejecutado
            var evento = context.MessageName.ToLower();

            Entity entity = (Entity)context.InputParameters["Target"];

            if (entity.LogicalName.Equals("dao_biblioteca") && entity.Attributes.Contains("dao_nombre"))
            {
                //Captura del nombre de la biblioteca que se digito 
                var bibliotecaDigitada = entity.Attributes["dao_nombre"].ToString().ToLower();

                ////QueryExpression
                var consultaBiblioteca = new QueryExpression("dao_biblioteca");
                consultaBiblioteca.NoLock = true;
                consultaBiblioteca.TopCount = 1;

                //Evento Create
                if (evento.Equals("create"))
                {
                    consultaBiblioteca.ColumnSet.AddColumns( "dao_nombre");
                    consultaBiblioteca.Criteria.AddCondition("dao_nombre", ConditionOperator.Equal, bibliotecaDigitada);

                    EntityCollection retrieveConsultaBiblioteca = service.RetrieveMultiple(consultaBiblioteca);

                    //// Si la consulta obtuvo al menos una entidad, hay existencia de un nombre ya utilizado
                    if (retrieveConsultaBiblioteca.Entities.Count > 0)
                    {
                        throw new InvalidPluginExecutionException("*** Nombre ya existente ***");
                    }
                }//Evento Update
                else if (evento.Equals("update"))
                {
                    consultaBiblioteca.ColumnSet.AddColumns("dao_bibliotecaid", "dao_nombre");
                    consultaBiblioteca.Criteria.AddCondition("dao_nombre", ConditionOperator.Equal, bibliotecaDigitada);

                    EntityCollection retrieveConsultaBiblioteca = service.RetrieveMultiple(consultaBiblioteca);

                    //// Si la consulta obtuvo al menos una entidad, hay existencia de un nombre ya utilizado
                    //   Se comparan los GUID obtenidos con el fin de verificar si la entidad contexto a modificar  
                    //   difiere de la entidad conusultada impidiendo asi el Update
                    if (retrieveConsultaBiblioteca.Entities.Count > 0)
                    {
                        ////Captura de Identificadores Unicos
                        // GUID de la entidad resultante de la consulta: Existencia de una biblioteca con el nombre digitado
                        Guid idConsultado = (Guid)retrieveConsultaBiblioteca.Entities[0].Attributes["dao_bibliotecaid"];
                        // GUID De la entidad contexto
                        Guid idEntity = (Guid)entity.Attributes["dao_bibliotecaid"];

                        if (idConsultado != idEntity)
                        {
                            throw new InvalidPluginExecutionException("*** La Biblioteca ya existe con ese nombre ***");
                        }
                        
                    }
                }
            }
        }
    }
}
