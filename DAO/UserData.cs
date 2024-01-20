using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoProtocol;
using MySql.Data.MySqlClient;

namespace EchoServer.DAO
{
    public class UserData
    {
        /// <summary>
        /// 如果用户名重复，会报错，不高兴改了
        /// </summary>
        /// <param name="park"></param>
        /// <returns></returns>
        public bool Logon(MainPark park, MySqlConnection mysqlCon)
        {
            string username = park.LoginPark.Username;
            string password = park.LoginPark.Password;
            try
            {
                //插入用户数据
                string sql = $"INSERT INTO userData (name, password) VALUES ('{username}','{password}')";
                MySqlCommand comd = new MySqlCommand(sql, mysqlCon);

                comd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public bool Login(MainPark park, MySqlConnection mysqlCon)
        {
            string username = park.LoginPark.Username;
            string password = park.LoginPark.Password;

            try
            {
                //查找用户数据
                string sql = $"SELECT * FROM userdata WHERE name='{username}' AND password = '{password}'";
                MySqlCommand comd = new MySqlCommand(sql, mysqlCon);
                MySqlDataReader reader = comd.ExecuteReader();
                bool result = reader.HasRows;
                reader.Close();
                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
