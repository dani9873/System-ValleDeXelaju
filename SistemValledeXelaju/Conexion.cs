using System.Data.OleDb;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SistemValledeXelaju
{
    internal class Conexion
    {
        public OleDbConnection con { get; private set; }

        public Conexion()
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Usuario\Documents\UMG\Proyecto\SistemValledeXelaju\bdValledeXelaju.accdb"; 
            con = new OleDbConnection(connectionString);
        }

        public void AbrirConexion()
        {
            try
            {
                if (con.State != System.Data.ConnectionState.Open)
                    con.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al abrir la conexión: " + ex.Message);
            }
        }

        public void CerrarConexion()
        {
            try
            {
                if (con.State != System.Data.ConnectionState.Closed)
                    con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cerrar la conexión: " + ex.Message);
            }
        }
        // Función para encriptar una cadena usando MD5
        public string EncriptarMD5(string cadena)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(cadena));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        // Función para desencriptar una cadena usando MD5 (No es posible desencriptar MD5, ya que es un hash unidireccional, pero se utiliza para verificar contraseñas).
        public bool VerificarMD5(string cadena, string hashGuardado)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                string hashCadena = EncriptarMD5(cadena);

                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                return comparer.Compare(hashCadena, hashGuardado) == 0;
            }
        }
    }
}