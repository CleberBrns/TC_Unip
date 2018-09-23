﻿using System.Collections.Generic;
using TCC_Unip.Models.Local;
using TCC_Unip.Models.Servico;
using TCC_Unip.ServicesAPI;
using TCC_Unip.Session;

namespace TCC_Unip.Services
{
    public class UsuarioService : Contracts.IServiceUsuario
    {
        readonly UsuarioServiceApi service = new UsuarioServiceApi();
        readonly UsuarioSession session = new UsuarioSession();
        readonly string sessionName = Constants.ConstSessions.listUsuarios;

        public ResultService<Usuario> Get(string email)
        {
            var result = new ResultService<Usuario>();

            var retornoSession = session.GetFromListSession(email, sessionName);

            if (retornoSession.Item2 && !string.IsNullOrEmpty(retornoSession.Item1.Email))
                result.value = retornoSession.Item1;
            else
                result.value = service.Get(email);

            if (string.IsNullOrEmpty(result.value.Email))
                result.errorMessage = "O Usuário não existe mais na base de dados do serviço!";

            return result;
        }

        public ResultService<List<Usuario>> List(bool getFromSession)
        {
            var result = new ResultService<List<Usuario>>();

            if (getFromSession)
            {
                var retornoSession = session.GetListFromSession(sessionName);
                /*Verifica se existia dados na session e se a mesma era válida.
                  Caso a mesma seja válida é passado para o retorno da pesquisa, mesmo que esteja vazia.
                  Caso não esteja criada, a busca é feita no serviço.*/
                if (retornoSession.Item2)                
                    result.value = retornoSession.Item1;                
                else
                    result.value = GetFromService();                    
            }
            else
            {
                var retorno = service.List();
                result.value = retorno;               
            }           

            return result;
        }

        private List<Usuario> GetFromService()
        {
            var list = service.List();
            session.AddListToSession(list, sessionName);
            return list;
        }

        public ResultService<bool> Save(Usuario model)
        {
            var result = new ResultService<bool>();

            var registroExistente = service.Get(model.Email);
            if (string.IsNullOrEmpty(registroExistente.Email))
            {
                var retorno = service.Save(model);
                result.value = retorno;

                if (result.value)
                    result.message = "Usuário salvo com sucesso!";
                else
                    result.errorMessage = "Falha ao salvar o Usuário!";
            }
            else
            {
                var retorno = service.Update(model);
                result.value = retorno;

                if (result.value)
                    result.message = "Usuário atualizado com sucesso!";
                else
                    result.errorMessage = "Falha ao atualizar o Usuário!";
            }

            return result;
        }

        public ResultService<bool> Delete(string cpf)
        {
            var result = new ResultService<bool>();

            var retorno = service.Delete(cpf);
            result.value = retorno;

            if (result.value)
                result.message = "Usuário excluído com sucesso!";
            else
                result.errorMessage = "Falha ao excluir o Usuário!";

            return result;
        }

        public ResultService<Usuario> Auth(Usuario model)
        {
            var result = new ResultService<Usuario>();

            var retorno = service.Auth(model);
            result.value = retorno;

            if (string.IsNullOrEmpty(result.value.Email))
                result.errorMessage = "Usuário Inválido ou Senha Incorreta";

            return result;
        }
    }
}