﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcUnip.Model.Cadastro
{
    public class FuncionarioModel
    {
        public int Id { get; set; }
        public bool Ativo { get; set; }
        public bool Excluido { get; set; }
        
        public int IdPessoa { get; set; }
        public PessoaModel Pessoa { get; set; }

        public int IdModalidade { get; set; }
        public ModalidadeModel Modalidade { get; set; }
    }
}
