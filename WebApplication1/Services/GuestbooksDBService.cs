using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class GuestbooksDBService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 查詢陣列資料
        //根據分頁以及搜尋來取得陣列資料的方法
        public List<Guestbooks> GetDataList(ForPaging Paging, string Search)
        {
            List<Guestbooks> DataList = new List<Guestbooks>();
            //Sql語法
            string sql = string.Empty;
            if (!string.IsNullOrWhiteSpace(Search))
            {
                //有搜尋條件時
                SetMaxPaging(Paging, Search);
                DataList = GetAllDataList(Paging, Search);
               // sql = $@" SELECT * FROM Guestbooks WHERE Name LIKE '%{Search}%' OR Content LIKE '%{Search}%' OR Reply LIKE '%{Search}%';";
            }
            else
            {
                //無搜尋條件時

                SetMaxPaging(Paging);
                DataList = GetAllDataList(Paging);
                //sql = $@" SELECT * FROM Guestbooks;";
            }
            return DataList;
        }
        #endregion

        //無搜尋值的搜尋資料方法
        #region 設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" SELECT * FROM Guestbooks;";
            //確保程式不會因執行錯誤而中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Row++;
                }
            }
            catch(Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數,以免有不正確值
            Paging.SetRightPage();
        }

        //有搜尋值的最大頁數方法
        public void SetMaxPaging(ForPaging Paging, string Search)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" SELECT * FROM Guestbooks WHERE Name LIKE '%{Search}%' OR Content LIKE '%{Search}%' OR Reply LIKE '%{Search}%';";
            //確保程式不會因執行錯誤而全部中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())  //獲得下一筆資料直到沒有
                {
                    Row++;
                }
            }
            catch (Exception e)
            {
                //丟錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //計算所需總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確頁數以免錯誤
            Paging.SetRightPage();
        }
        #endregion

        #region 搜尋資料方法
        //無搜尋值的搜尋資料方法
        public List<Guestbooks> GetAllDataList(ForPaging paging)
        {
            //宣告要回傳的搜尋資料為資料庫中的Guestbooks資料表
            List<Guestbooks> DataList = new List<Guestbooks>();
            //Sql語法
            string sql = $@" SELECT * FROM (SELECT row_number() OVER(order by Id) AS sort,
                          * FROM Guestbooks) m WHERE m.sort BETWEEN {(paging.NowPage - 1) * paging.ItemNum + 1} AND {paging.NowPage * paging.ItemNum};";
            //確保程式不會因執行錯誤而中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())//獲得下一筆直到沒有資料
                {
                    Guestbooks Data = new Guestbooks();
                    Data.Id = Convert.ToInt32(dr["Id"]);
                    Data.Name = dr["Name"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    //確認此留言是否回覆,不得空白
                    if (!string.IsNullOrWhiteSpace(dr["Reply"].ToString()))
                    {
                        Data.Reply = dr["Reply"].ToString();
                        Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                    }
                    DataList.Add(Data);
                }
            }
            catch(Exception e)
            {
                //丟錯
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳搜尋資料
            return DataList;
        }

        //有搜尋值的搜尋資料方法
        public List<Guestbooks> GetAllDataList(ForPaging paging,string Search)
        {
            //宣告要回傳的搜尋資料為資料庫中的Guestbooks資料表
            List<Guestbooks> DataList = new List<Guestbooks>();
            //Sql語法
            string sql = $@" SELECT * FROM (SELECT row_number() OVER(order by Id) AS sort,
                          * FROM Guestbooks WHERE Name LIKE '%{Search}%' OR Content LIKE '%{Search}%' OR Reply LIKE '%{Search}%') 
                          m WHERE m.sort BETWEEN {(paging.NowPage - 1) * paging.ItemNum + 1} AND {paging.NowPage * paging.ItemNum};";
            //確保程式不會因執行錯誤而中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())//獲得下一筆直到沒有資料
                {
                    Guestbooks Data = new Guestbooks();
                    Data.Id = Convert.ToInt32(dr["Id"]);
                    Data.Name = dr["Name"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    //確認此留言是否回覆,不得空白
                    if (!string.IsNullOrWhiteSpace(dr["Reply"].ToString()))
                    {
                        Data.Reply = dr["Reply"].ToString();
                        Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                    }
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                //丟錯
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳搜尋資料
            return DataList;
        }
        #endregion

        #region 新增資料
        //新增資料方法
        public void InsertGuestbooks(Guestbooks newData)
        {
            //Sql新增語法
            //設定新增時間為現在
            string sql = $@" INSERT INTO Guestbooks(Name,content,CreateTime) VALUES('{newData.Name}', '{newData.Content}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' );";
            //確保程式不會因執行錯誤而全部中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 查詢一筆資料
        //藉由編號取得單筆資料的方法
        public Guestbooks GetDataById(int Id)
        {
            Guestbooks Data = new Guestbooks();
            //Sql語法
            string sql = $@"SELECT * FROM Guestbooks WHERE Id ={Id}; ";
            //確保程式不會因執行錯誤而全部中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Id = Convert.ToInt32(dr["Id"]);
                Data.Name = dr["Name"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                //確認此留言是否回覆,不得空白
                if (!string.IsNullOrWhiteSpace(dr["Reply"].ToString()))
                {
                    Data.Reply = dr["Reply"].ToString();
                    Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                }
            }
            catch (Exception e)
            {
                //無資料
                Data = null;
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳根據編號所取得的資料
            return Data;
        }
        #endregion

        #region 修改留言
        //修改留言方法
        public void UpdateGuestbooks(Guestbooks UpdateData)
        {
            //Sql修改語法
            string sql = $@" UPDATE Guestbooks SET Name='{UpdateData.Name}',Content = '{UpdateData.Content}' WHERE Id ={UpdateData.Id};";
            //確保程式不會因執行錯誤而中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉連線
                conn.Close();
            }
        }
        #endregion

        #region 回覆留言
        //回覆留言方法
        public void ReplyGuestbooks(Guestbooks ReplyData)
        {
            //Sql修改語法
            //設定時間為現在
            string sql = $@" UPDATE Guestbooks SET Reply='{ReplyData.Reply}',ReplyTime = '{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' WHERE Id ={ReplyData.Id};";
            //確保程式不會因執行錯誤而中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉連線
                conn.Close();
            }
        }
        #endregion
        #region 檢查相關
        //修改資料判斷的方法
        public bool CheckUpdate(int Id)
        {
            //根據Id取得要修改的資料
            Guestbooks Data = GetDataById(Id);
            //判斷並回傳(是否有資料以及是否有回覆
            return (Data != null && Data.ReplyTime == null);
        }
        #endregion
        #region 刪除資料
        //刪除資料方法
        public void DeleteGuestbooks(int Id)
        {
            //Sql刪除語法
            //根據Id取得要刪除的資料
            string sql = $@" DELETE FROM Guestbooks WHERE Id ={Id};";
            //確保程式不會因執行錯誤而全部中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                //丟錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉連線
                conn.Close();
            }
        }
        #endregion
        
    }
}