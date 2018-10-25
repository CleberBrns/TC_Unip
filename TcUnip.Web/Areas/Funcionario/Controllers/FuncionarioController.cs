﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TcUnip.Web.Controllers;
using TcUnip.Web.Models.Local;
using TcUnip.Web.Services;
using TcUnip.Web.Session;
using TcUnip.Web.Util;

namespace TcUnip.Web.Areas.Funcionario.Controllers
{    
    public class FuncionarioController : BaseController
    {
        readonly Mensagens mensagens = new Mensagens();
        readonly FuncionarioService _service = new FuncionarioService();

        public ActionResult Listagem(bool getFromSession)
        {
            ValidaAutorizaoAcessoUsuario(Constants.ConstPermissoes.gerenciamento);

            var userInfo = GetUsuarioSession();

            if (!userInfo.Item2)
                return RedirectToAction("Login", "Login", new { area = "" });

            ViewBag.Usuario = userInfo.Item1;

            string msgExibicao = string.Empty;
            string msgAnalise = string.Empty;

            try
            {
                var list = new List<Models.Servico.Funcionario>();

                var resultService = _service.List(getFromSession);
                list = resultService.Value;

                msgExibicao = resultService.Message;
                msgAnalise = !resultService.Status ? "Falha!" : string.Empty;

                list = ConfiguraListaExibicao(list);

                return PartialView("_Listagem", list);
            }
            catch (Exception ex)
            {
                msgExibicao = Constants.Constants.msgFalhaAoListar;
                msgAnalise = ex.ToString();
            }

            var mensagensRetorno = mensagens.ConfiguraMensagemRetorno(msgExibicao, msgAnalise);
            return Json(new { mensagensRetorno }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModalCadastrar()
        {
            ValidaAutorizaoAcessoUsuario(Constants.ConstPermissoes.gerenciamento);

            ViewBag.Usuario = GetUsuarioSession().Item1;

            ViewBag.ListStatus = GetListStatus();
            ViewBag.ListModalidades = GetListModalidades();

            var model = new Models.Servico.Funcionario();
            var defaultObj = model.GetModelDefault();
            return PartialView("_Gerenciar", defaultObj);
        }

        [HttpGet]
        public ActionResult ModalEditar(string id)
        {
            ValidaAutorizaoAcessoUsuario(Constants.ConstPermissoes.gerenciamento);

            ViewBag.Usuario = GetUsuarioSession().Item1;

            string msgExibicao = string.Empty;
            string msgAnalise = string.Empty;

            try
            {
                ViewBag.ListStatus = GetListStatus();
                ViewBag.ListModalidades = GetListModalidades();

                var resultService = _service.Get(id);

                if (resultService.Status)
                    return PartialView("_Gerenciar", resultService.Value);
                else
                {
                    msgExibicao = resultService.Message;
                    msgAnalise = "Erro!";
                }

            }
            catch (Exception ex)
            {
                msgExibicao = Constants.Constants.msgFalhaAoCarregar;
                msgAnalise = ex.Message;
            }

            var mensagensRetorno = mensagens.ConfiguraMensagemRetorno(msgExibicao, msgAnalise);
            return Json(new { mensagensRetorno }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Salvar(Models.Servico.Funcionario model)
        {
            ValidaAutorizaoAcessoUsuario(Constants.ConstPermissoes.gerenciamento);

            string msgExibicao = string.Empty;
            string msgAnalise = string.Empty;

            try
            {
                var resultService = _service.Save(model);

                msgExibicao = resultService.Message;
                msgAnalise = !resultService.Status ? "Falha" : string.Empty;
            }
            catch (Exception ex)
            {
                msgExibicao = Constants.Constants.msgFalhaAoSalvar;
                msgAnalise = ex.Message;
            }

            var mensagensRetorno = mensagens.ConfiguraMensagemRetorno(msgExibicao, msgAnalise);
            return Json(new { mensagensRetorno }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Excluir(string id)
        {
            ValidaAutorizaoAcessoUsuario(Constants.ConstPermissoes.gerenciamento);

            string msgExibicao = string.Empty;
            string msgAnalise = string.Empty;

            try
            {
                var resultService = _service.Delete(id);

                msgExibicao = resultService.Message;
                msgAnalise = !resultService.Status ? "Falha" : string.Empty;
            }
            catch (Exception ex)
            {
                msgExibicao = Constants.Constants.msgFalhaAoExcluir;
                msgAnalise = ex.Message;
            }

            var mensagensRetorno = mensagens.ConfiguraMensagemRetorno(msgExibicao, msgAnalise);
            return Json(new { mensagensRetorno }, JsonRequestBehavior.AllowGet);
        }

        #region Métodos Privados

        private List<Models.Servico.Funcionario> ConfiguraListaExibicao(List<Models.Servico.Funcionario> list)
        {
            //Seleciona somente os itens há serem exibidos para melhor performance
            if (list != null && list.Count > 0)
            {
                list = list.Select(l =>
                    new Models.Servico.Funcionario
                    {
                        Nome = l.Nome,
                        Cpf = l.Cpf,
                        Status = l.Status,
                        Email = l.Email
                    }).ToList();
            }

            return list;
        }

        private List<DataSelectControl> GetListStatus()
        {
            var constants = new Constants.Constants();
            return constants.ListStatus();
        }

        private List<DataSelectControl> GetListModalidades()
        {
            var constants = new Constants.Constants();
            return constants.ListModalidades();
        }        

        #endregion
    }
}