﻿using eCommerceAPI.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eCommerceAPI.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private IDbConnection _connection;
        public UsuarioRepository()
        {
            _connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=eCommerce;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        public List<Usuario> Get()
        {
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "SELECT * FROM Usuarios";
                command.Connection = (SqlConnection)_connection;

                _connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    Usuario usuario = new Usuario();
                    usuario.Id = dataReader.GetInt32("Id");
                    usuario.Nome = dataReader.GetString("Nome");
                    usuario.Email = dataReader.GetString("Email");
                    usuario.Sexo = dataReader.GetString("Sexo");
                    usuario.RG = dataReader.GetString("RG");
                    usuario.CPF = dataReader.GetString("CPF");
                    usuario.NomeMae = dataReader.GetString("NomeMae");
                    usuario.SituacaoCadastro = dataReader.GetString("SituacaoCadastro");
                    usuario.DataCadastro = dataReader.GetDateTimeOffset(8);
                    usuarios.Add(usuario);
                }
            }
            finally
            {
                _connection.Close();

            }

            return usuarios;
        }

        public Usuario Get(int id)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "SELECT * FROM Usuarios u LEFT JOIN Contatos c ON c.UsuarioId = u.Id LEFT JOIN EnderecosEntrega ee ON ee.UsuarioId = u.Id LEFT JOIN UsuariosDepartamentos ud ON ud.UsuarioId = u.id LEFT JOIN Departamentos d ON ud.DepartamentoId = d.Id WHERE u.Id = @id";
                command.Parameters.AddWithValue("@id", id);
                command.Connection = (SqlConnection)_connection;

                _connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();

                Dictionary<int, Usuario> usuarios = new Dictionary<int, Usuario>();

                while (dataReader.Read())
                {
                    Usuario usuario = new Usuario();
                    if (!(usuarios.ContainsKey(dataReader.GetInt32(0))))
                    {
                        usuario.Id = dataReader.GetInt32(0);
                        usuario.Nome = dataReader.GetString("Nome");
                        usuario.Email = dataReader.GetString("Email");
                        usuario.Sexo = dataReader.GetString("Sexo");
                        usuario.RG = dataReader.GetString("RG");
                        usuario.CPF = dataReader.GetString("CPF");
                        usuario.NomeMae = dataReader.GetString("NomeMae");
                        usuario.SituacaoCadastro = dataReader.GetString("SituacaoCadastro");
                        usuario.DataCadastro = dataReader.GetDateTimeOffset(8);

                        Contato contato = new Contato();
                        contato.Id = dataReader.GetInt32(9);
                        contato.UsuarioId = usuario.Id;
                        //contato.Telefone = dataReader.GetString("Telefone");
                        //contato.Celular = dataReader.GetString("Celular");

                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("Telefone")))
                        {
                            contato.Telefone = dataReader.GetString("Telefone");
                        }

                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("Celular")))
                        {
                            contato.Celular = dataReader.GetString("Celular");
                        }

                        usuario.Contato = contato;

                        usuarios.Add(usuario.Id, usuario);
                    } 
                    else
                    {
                        usuario = usuarios[dataReader.GetInt32(0)];
                    }

                    EnderecoEntrega enderecoEntrega = new EnderecoEntrega();
                    enderecoEntrega.Id = dataReader.GetInt32(13);
                    enderecoEntrega.UsuarioId = usuario.Id;
                    enderecoEntrega.NomeEndereco = dataReader.GetString("NomeEndereco");
                    enderecoEntrega.CEP = dataReader.GetString("CEP");
                    enderecoEntrega.Estado = dataReader.GetString("Estado");
                    enderecoEntrega.Cidade = dataReader.GetString("Cidade");
                    enderecoEntrega.Bairro = dataReader.GetString("Bairro");
                    enderecoEntrega.Endereco = dataReader.GetString("Endereco");
                    enderecoEntrega.Numero = dataReader.GetString("Numero");
                    enderecoEntrega.Complemento = dataReader.GetString("Complemento");

                    usuario.EnderecosEntrega = (usuario.EnderecosEntrega == null) ? new List<EnderecoEntrega>() : usuario.EnderecosEntrega;

                    if(usuario.EnderecosEntrega.FirstOrDefault(ee => ee.Id == enderecoEntrega.Id) == null)
                    {
                        usuario.EnderecosEntrega.Add(enderecoEntrega);
                    }

                    Departamento departamento = new Departamento();
                    departamento.Id = dataReader.GetInt32(26);
                    departamento.Nome = dataReader.GetString(27);

                    usuario.Departamentos = (usuario.Departamentos == null) ? new List<Departamento>() : usuario.Departamentos;

                    if(usuario.Departamentos.FirstOrDefault(d => d.Id == departamento.Id) == null)
                    {
                        usuario.Departamentos.Add(departamento);
                    }
                }
            
                return usuarios[usuarios.Keys.First()];
            }
            catch
            {
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Insert(Usuario usuario)
        {
            _connection.Open();

            SqlTransaction transaction = (SqlTransaction)_connection.BeginTransaction();
            try
            {
                #region Usuario
                SqlCommand command = new SqlCommand();
                command.Transaction = transaction;
                command.Connection = (SqlConnection)_connection;

                command.CommandText = "INSERT INTO Usuarios (Nome, Email, Sexo, RG, CPF, NomeMae, SituacaoCadastro, DataCadastro) Values(@Nome, @Email, @Sexo, @RG, @CPF, @NomeMae, @SituacaoCadastro, @DataCadastro);SELECT CAST(scope_identity() AS int)";
                command.Parameters.AddWithValue("@Nome", usuario.Nome);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Sexo", usuario.Sexo);
                command.Parameters.AddWithValue("@RG", usuario.RG);
                command.Parameters.AddWithValue("@CPF", usuario.CPF);
                command.Parameters.AddWithValue("@NomeMae", usuario.NomeMae);
                command.Parameters.AddWithValue("@SituacaoCadastro", usuario.SituacaoCadastro);
                command.Parameters.AddWithValue("@DataCadastro", usuario.DataCadastro);
                
                usuario.Id = (int)command.ExecuteScalar();
                #endregion

                #region Contatos
                command.CommandText = "INSERT INTO Contatos (UsuarioId, Telefone, Celular) VALUES (@UsuarioId, @Telefone, @Celular); SELECT CAST(scope_identity() AS int)";
                command.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                command.Parameters.AddWithValue("@Telefone", usuario.Contato.Telefone);
                command.Parameters.AddWithValue("@Celular", usuario.Contato.Celular);

                usuario.Contato.UsuarioId = usuario.Id;
                usuario.Contato.Id = (int)command.ExecuteScalar();
                #endregion

                #region EnderecosEntrega
                foreach (var endereco in usuario.EnderecosEntrega)
                {
                    command = new SqlCommand();
                    command.Connection = (SqlConnection)_connection;
                    command.Transaction = transaction;

                    command.CommandText = "INSERT INTO EnderecosEntrega(UsuarioId, NomeEndereco, CEP, Cidade, Estado, Bairro, Endereco, Numero, Complemento) VALUES (@UsuarioId, @NomeEndereco, @CEP, @Cidade, @Estado, @Bairro, @Endereco, @Numero, @Complemento);SELECT CAST(scope_identity() AS int)";
                    command.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                    command.Parameters.AddWithValue("@NomeEndereco", endereco.NomeEndereco);
                    command.Parameters.AddWithValue("@CEP", endereco.CEP);
                    command.Parameters.AddWithValue("@Cidade", endereco.Cidade);
                    command.Parameters.AddWithValue("@Estado", endereco.Estado);
                    command.Parameters.AddWithValue("@Bairro", endereco.Bairro);
                    command.Parameters.AddWithValue("@Endereco", endereco.Endereco);
                    command.Parameters.AddWithValue("@Numero", endereco.Numero);
                    command.Parameters.AddWithValue("@Complemento", endereco.Complemento);

                    endereco.Id = (int)command.ExecuteScalar();
                    endereco.UsuarioId = usuario.Id;
                }
                #endregion

                #region Departamentos
                foreach (var departamento in usuario.Departamentos)
                {
                    command = new SqlCommand();
                    command.Connection = (SqlConnection)_connection;
                    command.Transaction = transaction;

                    command.CommandText = "INSERT INTO UsuariosDepartamentos (UsuarioId, DepartamentoId) VALUES (@UsuarioId, @DepartamentoId)";
                    command.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                    command.Parameters.AddWithValue("@DepartamentoId", departamento.Id);

                    command.ExecuteNonQuery();
                }
                #endregion

                transaction.Commit();
            }
            catch
            {
                try
                {
                    transaction.Rollback();
                }
                catch 
                { 
                    //Tratativa de erro para desfazer transaction
                }

                throw new Exception("Erro ao tentar inserir os dados.");
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Update(Usuario usuario)
        {
            _connection.Open();
            SqlTransaction transaction = (SqlTransaction)_connection.BeginTransaction();
            try
            {
                #region Usuario
                SqlCommand command = new SqlCommand();
                command.Connection = (SqlConnection)_connection;
                command.Transaction= transaction;

                command.CommandText = "UPDATE Usuarios SET Nome = @Nome, Email = @Email, Sexo = @Sexo, RG = @RG, CPF = @CPF, NomeMae = @NomeMae, SituacaoCadastro = @SituacaoCadastro, DataCadastro = @DataCadastro WHERE Id = @Id";
                command.Parameters.AddWithValue("@Nome", usuario.Nome);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Sexo", usuario.Sexo);
                command.Parameters.AddWithValue("@RG", usuario.RG);
                command.Parameters.AddWithValue("@CPF", usuario.CPF);
                command.Parameters.AddWithValue("@NomeMae", usuario.NomeMae);
                command.Parameters.AddWithValue("@SituacaoCadastro", usuario.SituacaoCadastro);
                command.Parameters.AddWithValue("@DataCadastro", usuario.DataCadastro);
                command.Parameters.AddWithValue("@Id", usuario.Id);

                command.ExecuteNonQuery();
                #endregion

                #region Contato
                command = new SqlCommand();
                command.Connection = (SqlConnection)_connection;
                command.Transaction = transaction;
                command.CommandText = "UPDATE Contatos SET UsuarioId = @UsuarioId, Telefone = @Telefone, Celular = @Celular WHERE Id = @Id";

                command.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                command.Parameters.AddWithValue("@Telefone", usuario.Contato.Telefone);
                command.Parameters.AddWithValue("@Celular", usuario.Contato.Celular);
                command.Parameters.AddWithValue("@Id", usuario.Contato.Id);

                command.ExecuteNonQuery();
                #endregion

                #region EnderecosEntrega
                command = new SqlCommand();
                command.Connection = (SqlConnection)_connection;
                command.Transaction = transaction;
                command.CommandText = "DELETE FROM EnderecosEntrega WHERE UsuarioId = @UsuarioId";
                command.Parameters.AddWithValue("@UsuarioId", usuario.Id);

                command.ExecuteNonQuery();

                foreach (var endereco in usuario.EnderecosEntrega)
                {
                    command = new SqlCommand();
                    command.Connection = (SqlConnection)_connection;
                    command.Transaction = transaction;

                    command.CommandText = "INSERT INTO EnderecosEntrega(UsuarioId, NomeEndereco, CEP, Cidade, Estado, Bairro, Endereco, Numero, Complemento) VALUES (@UsuarioId, @NomeEndereco, @CEP, @Cidade, @Estado, @Bairro, @Endereco, @Numero, @Complemento);SELECT CAST(scope_identity() AS int)";
                    command.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                    command.Parameters.AddWithValue("@NomeEndereco", endereco.NomeEndereco);
                    command.Parameters.AddWithValue("@CEP", endereco.CEP);
                    command.Parameters.AddWithValue("@Cidade", endereco.Cidade);
                    command.Parameters.AddWithValue("@Estado", endereco.Estado);
                    command.Parameters.AddWithValue("@Bairro", endereco.Bairro);
                    command.Parameters.AddWithValue("@Endereco", endereco.Endereco);
                    command.Parameters.AddWithValue("@Numero", endereco.Numero);
                    command.Parameters.AddWithValue("@Complemento", endereco.Complemento);

                    endereco.Id = (int)command.ExecuteScalar();
                    endereco.UsuarioId = usuario.Id;
                }
                #endregion

                #region Departamentos
                command = new SqlCommand();
                command.Connection = (SqlConnection)_connection;
                command.Transaction = transaction;
                command.CommandText = "DELETE FROM UsuariosDepartamentos WHERE UsuarioId = @UsuarioId";
                command.Parameters.AddWithValue("@UsuarioId", usuario.Id);

                command.ExecuteNonQuery();

                foreach (var departamento in usuario.Departamentos)
                {
                    command = new SqlCommand();
                    command.Connection = (SqlConnection)_connection;
                    command.Transaction = transaction;

                    command.CommandText = "INSERT INTO UsuariosDepartamentos (UsuarioId, DepartamentoId) VALUES (@UsuarioId, @DepartamentoId)";
                    command.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                    command.Parameters.AddWithValue("@DepartamentoId", departamento.Id);

                    command.ExecuteNonQuery();
                }
                #endregion

                transaction.Commit();
            }
            catch
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {

                }

                throw new Exception("Erro: não conseguimos atualizar os dados!");
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Delete(int id)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "DELETE FROM Usuarios WHERE Id = @Id";
                command.Connection = (SqlConnection)_connection;
                command.Parameters.AddWithValue("@Id", id);

                _connection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }
        }

    }
}
