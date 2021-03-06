﻿using GL.Command.DBUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GL.Data.BLL
{
    public class UnAnnouncementBLL
    {

        public static readonly string database1 = PubConstant.GetConnectionString("database1");
        public static readonly string database2 = PubConstant.GetConnectionString("database2");
        public static readonly string database3 = PubConstant.GetConnectionString("database3");


        public static string GetModel()
        {
            return DAL.UnAnnouncementDAL.GetModel();
        }

        public static int Update(string value)
        {
            return DAL.UnAnnouncementDAL.Update(value);
        }

        public static int UpdateGameAnnouncement(List<GL.Data.Model.GameAnnouncement> games)
        {
            string sql = "delete from "+ database2 + @".Unrelated_GAnnouncement;";
            foreach (var item in games)
            {
                string title = item.Title;
                string content = item.Content;
                int indexNo = item.IndexNo;
                sql =sql+ @"
insert into "+ database2 + @".Unrelated_GAnnouncement(Title, Content, IndexNo) VALUES 
('"+item.Title+"', '"+item.Content+"', "+item.IndexNo+");";

            }



            return DAL.UnAnnouncementDAL.UpdateGameAnnouncement(sql);
        }

        public static string GetGameAnnouncement()
        {
            string sql = "select * from "+ database2 + @".Unrelated_GAnnouncement;";
            List<GL.Data.Model.GameAnnouncement> models = DAL.UnAnnouncementDAL.GetGameAnnouncement(sql);
            string resl = "";
            foreach (GL.Data.Model.GameAnnouncement item in models)
            {
                resl = resl + item.Title +"\n"+ item.Content+"\n\n";
            }


            return resl;
        }

    }
}
