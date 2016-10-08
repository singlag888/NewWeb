﻿using GL.Command.DBUtility;
using GL.Common;
using GL.Data.DAL;
using GL.Data.Model;
using GL.Data.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;

namespace GL.Data.BLL
{
    public class GameDataBLL
    {

        public static readonly string sqlconnectionStringForRecord = PubConstant.GetConnectionString("ConnectionStringForGameRecord");
        public static readonly string sqlconnectionString = PubConstant.GetConnectionString("ConnectionStringForGameData");

        public static PagedList<ScalePot> GetListByPageForScalePot(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

           pq.RecordCount = DAL.PagedListDAL<LandGameRecord>.GetRecordCount(string.Format(@"select count(0) from record.BG_ScalePot where CreateTime between '{0}' and '{1}'  {2} ", model.StartDate, model.ExpirationDate,model.SearchExt==""?" ": " and  BoardNo = '"+model.SearchExt+"'"), sqlconnectionStringForRecord);
           pq.Sql = string.Format(@"select * from record.BG_ScalePot where CreateTime between '{2}' and '{3}' {4}  order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.SearchExt == "" ? " " : " and  BoardNo = '" + model.SearchExt + "'");
          

            PagedList<ScalePot> obj = new PagedList<ScalePot>(DAL.PagedListDAL<ScalePot>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }



        public static PagedList<ScalePot> GetListByPageForTexProPot(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

            pq.RecordCount = DAL.PagedListDAL<LandGameRecord>.GetRecordCount(string.Format(@"select count(0) from record.BG_TexProPot where CreateTime between '{0}' and '{1}'  {2} ", model.StartDate, model.ExpirationDate, model.SearchExt == "" ? " " : " and  BoardNo = '" + model.SearchExt + "'"), sqlconnectionStringForRecord);
            pq.Sql = string.Format(@"select * from record.BG_TexProPot where CreateTime between '{2}' and '{3}' {4}  order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.SearchExt == "" ? " " : " and  BoardNo = '" + model.SearchExt + "'");


            PagedList<ScalePot> obj = new PagedList<ScalePot>(DAL.PagedListDAL<ScalePot>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }


        public static PagedList<MiniGameSum> GetMiniGameSum(GameRecordView model) {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

            pq.RecordCount = DAL.PagedListDAL<MiniGameSum>.GetRecordCount(string.Format(@"

select count(*) from (
select date(a.CreateTime)
from BG_TexasMiniGameRecord as a
where a.CreateTime>='{0}' and a.CreateTime<'{1}'
GROUP BY date(a.CreateTime)
)as a;


", 
model.StartDate,
model.ExpirationDate
), sqlconnectionStringForRecord);

            pq.Sql = string.Format(@"
select a.*,b.XiaoHao,c.ChanChu,d.ClickNum,d.ClickCount,d.Active from (
select 
date(CreateTime) as CreateTime, 
sum(case  when ABS( a.ChipUse)<=100 then 1 else 0 end ) AS M0_100,
sum(case  when ABS(a.ChipUse)>=101 and ABS(a.ChipUse)<=1000 then 1 else 0 end ) AS M101_1000,
sum(case  when ABS(a.ChipUse)>=1001 and ABS(a.ChipUse)<=5000 then 1 else 0 end ) AS M1001_5000,
sum(case  when ABS(a.ChipUse)>=5001 and ABS(a.ChipUse)<=20000 then 1 else 0 end ) AS M5001_2W,
sum(case  when ABS(a.ChipUse)>=20001 and ABS(a.ChipUse)<=100000 then 1 else 0 end ) AS M2W01_10W,
sum(case  when ABS(a.ChipUse)>=100001 and ABS(a.ChipUse)<=1000000 then 1 else 0 end ) AS M10W01_100W,
sum(case  when ABS(a.ChipUse)>=1000001 then 1 else 0 end ) AS MMore100W
from BG_TexasMiniGameRecord as a
where a.CreateTime>='{2}' and a.CreateTime<'{3}'
GROUP BY date(a.CreateTime)
limit {0}, {1} ) a 

LEFT JOIN 
(
SELECT date(RecordTime) as CreateTime,SUM(Chip) as XiaoHao from record.Clearing_UserMoneyRecord 
WHERE RecordTime>='{2}' and RecordTime<'{3}' and RecordType = 199 
GROUP BY date(RecordTime)  
) b on a.CreateTime = b.CreateTime
LEFT JOIN 
(
SELECT date(RecordTime) as CreateTime,SUM(Chip) as ChanChu from record.Clearing_UserMoneyRecord 
WHERE RecordTime>='{2}' and RecordTime<'{3}' and RecordType = 200 
GROUP BY date(RecordTime)  
) c on a.CreateTime = c.CreateTime

LEFT JOIN 
(
select a.CreateTime,SUM(Active) as Active, SUM(ClickNum) as ClickNum, SUM(ClickCount) as ClickCount from (
   select 0 Active ,count(distinct  UserID ) ClickNum ,sum(ClickCount) ClickCount ,DATE(a.CreateTime ) as CreateTime
   from BG_ClickRecord a join 515game.Role b on a.UserID = b.ID and b.isfreeze = 0 
   where a.ModeluID = 24 and a.CreateTime >= '{2}' and a.CreateTime < '{3}' 
         and a.SiteID in (1,2) group by DATE(a.CreateTime) union ALL
		select Sum(SumValue),0,0,DATE(CountDate)
		from Clearing_Day where TypeID = 103
		and CountDate>='{2}' and CountDate<'{3}' and Agent!=10010
		GROUP BY DATE(CountDate)
) as a
GROUP BY CreateTime

) d on a.CreateTime = d.CreateTime



", 
pq.StartRowNumber,
pq.PageSize,
model.StartDate, 
model.ExpirationDate);


            PagedList<MiniGameSum> obj = new PagedList<MiniGameSum>(DAL.PagedListDAL<MiniGameSum>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }



        public static PagedList<ThanksGiving> GetListByPageForThanksGiving(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
        //    pq.CurrentPage = model.Page;
            pq.CurrentPage =1;
            pq.PageSize = 100000;

            //            pq.RecordCount = DAL.PagedListDAL<ThanksGiving>.GetRecordCount(string.Format(@"
            //select count(0) from record.BG_TexProPot where CreateTime between '{0}' and '{1}'  {2}
            //", model.StartDate, model.ExpirationDate, model.SearchExt == "" ? " " : " and  BoardNo = '" + model.SearchExt + "'"), sqlconnectionStringForRecord);


            pq.RecordCount = 100000;

           pq.Sql = string.Format(@"
select 
CountDate as CountTime, 
sum(case a.TypeID when 3 then SumCount else 0 end ) AS ClickCount,
sum(case a.TypeID when 3 then SumNum else 0 end ) AS ClickNum,
sum(case a.TypeID when 4 then SumNum else 0 end ) AS Num30,
sum(case a.TypeID when 5 then SumNum else 0 end ) AS Num68,
sum(case a.TypeID when 6 then SumNum else 0 end ) AS Num128,
sum(case a.TypeID when 7 then SumNum else 0 end ) AS Num328,
sum(case a.TypeID when 8 then SumNum else 0 end ) AS Num300,
sum(case a.TypeID when 9 then SumNum else 0 end ) AS Num500,
sum(case a.TypeID when 10 then SumNum else 0 end ) AS Num700,
sum(case a.TypeID when 11 then SumNum else 0 end ) AS Num1000,
sum(case a.TypeID when 12 then SumNum else 0 end ) AS Num1500
from Clearing_ThkgRebate as a
where a.TypeID>=3 and a.TypeID<=12 and  CountDate >='{2}' and CountDate<'{3}'  {4}
GROUP BY CountDate

", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Channels <=0  ? " " : " AND agent =   " + model.Channels + "");


            PagedList<ThanksGiving> obj = new PagedList<ThanksGiving>(DAL.PagedListDAL<ThanksGiving>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }


        public static List<ThanksRank> GetThanksRankList(GameRecordView model) {
            IEnumerable<ThanksRank> ienums = GameDataDAL.GetThanksRankList(model);
            if (ienums == null)
            {
                return new List<ThanksRank>();
            }
            else {
                return ienums.ToList();
            }
        }

        // GetMFestivalRankList
        public static List<ThanksRank> GetMFestivalRankList(GameRecordView model)
        {
            IEnumerable<ThanksRank> ienums = GameDataDAL.GetMFestivalRankList(model);
            if (ienums == null)
            {
                return new List<ThanksRank>();
            }
            else
            {
                return ienums.ToList();
            }
        }

        public static PagedList<ThanksGiving> GetListByPageForMFestival(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            //    pq.CurrentPage = model.Page;
            pq.CurrentPage = 1;
            pq.PageSize = 100000;

            //            pq.RecordCount = DAL.PagedListDAL<ThanksGiving>.GetRecordCount(string.Format(@"
            //select count(0) from record.BG_TexProPot where CreateTime between '{0}' and '{1}'  {2}
            //", model.StartDate, model.ExpirationDate, model.SearchExt == "" ? " " : " and  BoardNo = '" + model.SearchExt + "'"), sqlconnectionStringForRecord);


            pq.RecordCount = 100000;

            pq.Sql = string.Format(@"
select 
CountDate as CountTime, 
sum(case a.TypeID when 23 then SumCount else 0 end ) AS ClickCount,
sum(case a.TypeID when 23 then SumNum else 0 end ) AS ClickNum,
sum(case a.TypeID when 26 then SumNum else 0 end ) AS Num30,
sum(case a.TypeID when 27 then SumNum else 0 end ) AS Num68,
sum(case a.TypeID when 28 then SumNum else 0 end ) AS Num128,
sum(case a.TypeID when 29 then SumNum else 0 end ) AS Num328,
sum(case a.TypeID when 25 then SumNum else 0 end ) AS Other
from Clearing_ThkgRebate as a
where  a.TypeID in (23,26,27,28,29,25) and  CountDate >='{2}' and CountDate<'{3}'  {4}
GROUP BY CountDate

", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Channels <= 0 ? " " : " AND agent =   " + model.Channels + "");


            PagedList<ThanksGiving> obj = new PagedList<ThanksGiving>(DAL.PagedListDAL<ThanksGiving>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }


        public static PagedList<ThanksBaoGuang> GetListByPageForThanksBaoGuang(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

            pq.RecordCount = DAL.PagedListDAL<ThanksBaoGuang>.GetRecordCount(string.Format(@"select count(0) from record.BG_TexProPot where CreateTime between '{0}' and '{1}'  {2} ", model.StartDate, model.ExpirationDate, model.SearchExt == "" ? " " : " and  BoardNo = '" + model.SearchExt + "'"), sqlconnectionStringForRecord);
            pq.Sql = string.Format(@"select * from record.BG_TexProPot where CreateTime between '{2}' and '{3}' {4}  order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.SearchExt == "" ? " " : " and  BoardNo = '" + model.SearchExt + "'");


            PagedList<ThanksBaoGuang> obj = new PagedList<ThanksBaoGuang>(DAL.PagedListDAL<ThanksBaoGuang>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }



        //TexasProAccording
        public static PagedList<TexasProAccording> GetListByPageForTexasProAccording(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

            pq.RecordCount = DAL.PagedListDAL<TexasProAccording>.GetRecordCount(string.Format(@"
select count(*) from record.BG_BankRecord
where CreateTime >='{0}' and CreateTime<'{1}' 
{2} {3} ",
model.StartDate,
model.ExpirationDate, 
model.SearchExt == "" ? " " : "AND ( UserID ="+model.SearchExt+" or RoundID='"+model.SearchExt+"' ) ",
model.GametypeS == "" ? " " : "AND ( Type in ("+model.GametypeS+") )"), sqlconnectionStringForRecord);

            pq.Sql = string.Format(@"
select * from record.BG_BankRecord
where CreateTime >='{0}' and CreateTime<'{1}' 
{2}  {3} order by CreateTime desc  limit {4}, {5} ",
model.StartDate,
model.ExpirationDate,
model.SearchExt == "" ? " " : "AND ( UserID =" + model.SearchExt + " or RoundID='" + model.SearchExt + "' )",
model.GametypeS == "" ? " " : "AND ( Type in (" + model.GametypeS + ") )",
 pq.StartRowNumber, 
 pq.PageSize);


            PagedList<TexasProAccording> obj = new PagedList<TexasProAccording>(DAL.PagedListDAL<TexasProAccording>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }

        /// <summary>
        /// 开心翻翻乐的牌局记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static PagedList<MiniGameRecord> GetListByPageForMiniGameRecord(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

            pq.RecordCount = DAL.PagedListDAL<MiniGameRecord>.GetRecordCount(string.Format(@"
select count(*) from record.BG_TexasMiniGameRecord
where CreateTime >='{0}' and CreateTime<'{1}' 
{2}  ",
model.StartDate,
model.ExpirationDate,
model.SearchExt == "" ? " " : "AND ( UserID =" + model.SearchExt +" ) "), sqlconnectionStringForRecord);

            pq.Sql = string.Format(@"
select a.*,b.NickName from record.BG_TexasMiniGameRecord as a,515game.Role as b
where a.CreateTime >='{0}' and a.CreateTime<'{1}' and a.UserID = b.ID 
{2}   order by a.CreateTime desc  limit {3}, {4} ",
model.StartDate,
model.ExpirationDate,
model.SearchExt == "" ? " " : "AND ( a.UserID =" + model.SearchExt + " )",
 pq.StartRowNumber,
 pq.PageSize);


            PagedList<MiniGameRecord> obj = new PagedList<MiniGameRecord>(DAL.PagedListDAL<MiniGameRecord>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }



        public static PagedList<BoardDist> GetListByPageForBoardDist(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage =1;
            pq.PageSize = 10000000;

          //  pq.RecordCount = DAL.PagedListDAL<BoardDist>.GetRecordCount(string.Format(@"select count(0) from record.BG_TexProPot where CreateTime between '{0}' and '{1}'  {2} ", model.StartDate, model.ExpirationDate, model.SearchExt == "" ? " " : " and  BoardNo = '" + model.SearchExt + "'"), sqlconnectionStringForRecord);
            pq.RecordCount = 10000000;
            pq.Sql = string.Format(@"
select date_add('{2}' ,interval o.id day) CreateTime ,NewAddUser NewAddUser ,BoardTotal BoardTotal 
  ,ifnull(Board0 ,0) Board0 ,ifnull(Board1 ,0) Board1 ,ifnull(Board2 ,0) Board2 ,ifnull(Board3 ,0) Board3 ,ifnull(Board4 ,0) Board4 ,ifnull(Board5 ,0) Board5
  ,ifnull(Board6_10 ,0) Board6_10 ,ifnull(Board11_15 ,0) Board11_15 ,ifnull(Board16_20 ,0) Board16_20 ,ifnull(Board21_25 ,0) Board21_25 ,ifnull(Board26_30 ,0) Board26_30
  ,ifnull(Board31_35 ,0) Board31_35 ,ifnull(Board36_40 ,0) Board36_40 ,ifnull(Board41_45 ,0) Board41_45 ,ifnull(Board46_50 ,0) Board46_50 ,ifnull(BoardMore50 ,0) BoardMore50
from record.S_Ordinal o left join(
select CountDate ,sum(UsersCount) NewAddUser ,sum(BoardCount) BoardTotal 
  ,sum(case when BoardCount = 0 then UsersCount else 0 end) Board0 
  ,sum(case when BoardCount = 1 then UsersCount else 0 end) Board1 
  ,sum(case when BoardCount = 2 then UsersCount else 0 end) Board2 
  ,sum(case when BoardCount = 3 then UsersCount else 0 end) Board3 
  ,sum(case when BoardCount = 4 then UsersCount else 0 end) Board4 
  ,sum(case when BoardCount = 5 then UsersCount else 0 end) Board5 
  ,sum(case when BoardCount>5 and BoardCount<11 then  UsersCount else 0 end) Board6_10
  ,sum(case when BoardCount>10 and BoardCount<16 then UsersCount else 0 end) Board11_15 
  ,sum(case when BoardCount>15 and BoardCount<21 then UsersCount else 0 end) Board16_20 
  ,sum(case when BoardCount>20 and BoardCount<26 then UsersCount else 0 end) Board21_25 
  ,sum(case when BoardCount>25 and BoardCount<31 then UsersCount else 0 end) Board26_30 
  ,sum(case when BoardCount>30 and BoardCount<36 then UsersCount else 0 end) Board31_35 
  ,sum(case when BoardCount>35 and BoardCount<41 then UsersCount else 0 end) Board36_40 
  ,sum(case when BoardCount>40 and BoardCount<46 then UsersCount else 0 end) Board41_45 
  ,sum(case when BoardCount>45 and BoardCount<51 then UsersCount else 0 end) Board46_50
  ,sum(case when BoardCount>50 then UsersCount else 0 end) BoardMore50  
from record.Clearing_GameKeep a 
where a.CountDate >= '{2}' and CountDate <'{3}' and UserType = {0} and (GameType = {1} or GameType = 0) 
  and Agent = case when {4} = 0 then Agent else {4} end group by CountDate
)b on CountDate = date_add('{2}' ,interval o.id day)
where o.id < datediff('{3}' ,'{2}') order by 1 desc ;
", 
model.UserType,
model.Gametype, 
model.StartDate,
model.ExpirationDate, 
model.Channels);


            PagedList<BoardDist> obj = new PagedList<BoardDist>(DAL.PagedListDAL<BoardDist>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }


        public static PagedList<BoardDist> GetListByPageForBoardRate(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = 1;
            pq.PageSize = 10000000;

            //  pq.RecordCount = DAL.PagedListDAL<BoardDist>.GetRecordCount(string.Format(@"select count(0) from record.BG_TexProPot where CreateTime between '{0}' and '{1}'  {2} ", model.StartDate, model.ExpirationDate, model.SearchExt == "" ? " " : " and  BoardNo = '" + model.SearchExt + "'"), sqlconnectionStringForRecord);
            pq.RecordCount = 10000000;
            pq.Sql = string.Format(@"


select date_add('{2}' ,interval o.id day) CreateTime ,NewAddUser NewAddUser 
  ,ifnull(case when Board0 > 0 then Keep0/Board0 else 0 end,0)*100 Board0
  ,ifnull(case when Board1 > 0 then Keep1/Board1 else 0 end,0)*100 Board1
  ,ifnull(case when Board2 > 0 then Keep2/Board2 else 0 end,0)*100 Board2
  ,ifnull(case when Board3 > 0 then Keep3/Board3 else 0 end,0)*100 Board3
  ,ifnull(case when Board4 > 0 then Keep4/Board4 else 0 end,0)*100 Board4
  ,ifnull(case when Board5 > 0 then Keep5/Board5 else 0 end,0)*100 Board5
  ,ifnull(case when Board6_10 > 0 then Keep6_10/Board6_10 else 0 end,0)*100 Board6_10
  ,ifnull(case when Board11_15 > 0 then Keep11_15/Board11_15 else 0 end,0)*100 Board11_15
  ,ifnull(case when Board16_20 > 0 then Keep16_20/Board16_20 else 0 end,0)*100 Board16_20
  ,ifnull(case when Board21_25 > 0 then Keep21_25/Board21_25 else 0 end,0)*100 Board21_25
  ,ifnull(case when Board26_30 > 0 then Keep26_30/Board26_30 else 0 end,0)*100 Board26_30
  ,ifnull(case when Board31_35 > 0 then Keep31_35/Board31_35 else 0 end,0)*100 Board31_35
  ,ifnull(case when Board36_40 > 0 then Keep36_40/Board36_40 else 0 end,0)*100 Board36_40
  ,ifnull(case when Board41_45 > 0 then Keep41_45/Board41_45 else 0 end,0)*100 Board41_45
  ,ifnull(case when Board46_50 > 0 then Keep46_50/Board46_50 else 0 end,0)*100 Board46_50
  ,ifnull(case when BoardMore50 > 0 then KeepMore50/BoardMore50 else 0 end,0)*100 BoardMore50
from record.S_Ordinal o left join(
select CountDate ,sum(UsersCount) NewAddUser ,sum(BoardCount) BoardTotal 
  ,sum(case when BoardCount = 0 then UsersCount else 0 end) Board0 ,sum(case when BoardCount = 0 then UserKeep else 0 end) Keep0
  ,sum(case when BoardCount = 1 then UsersCount else 0 end) Board1 ,sum(case when BoardCount = 1 then UserKeep else 0 end) Keep1
  ,sum(case when BoardCount = 2 then UsersCount else 0 end) Board2 ,sum(case when BoardCount = 2 then UserKeep else 0 end) Keep2
  ,sum(case when BoardCount = 3 then UsersCount else 0 end) Board3 ,sum(case when BoardCount = 3 then UserKeep else 0 end) Keep3
  ,sum(case when BoardCount = 4 then UsersCount else 0 end) Board4 ,sum(case when BoardCount = 4 then UserKeep else 0 end) Keep4
  ,sum(case when BoardCount = 5 then UsersCount else 0 end) Board5 ,sum(case when BoardCount = 5 then UserKeep else 0 end) Keep5
  ,sum(case when BoardCount>5 and BoardCount<11 then  UsersCount else 0 end) Board6_10  ,sum(case when BoardCount>5 and BoardCount<11 then  UserKeep else 0 end) Keep6_10
  ,sum(case when BoardCount>10 and BoardCount<16 then UsersCount else 0 end) Board11_15 ,sum(case when BoardCount>10 and BoardCount<16 then UserKeep else 0 end) Keep11_15 
  ,sum(case when BoardCount>15 and BoardCount<21 then UsersCount else 0 end) Board16_20 ,sum(case when BoardCount>15 and BoardCount<21 then UserKeep else 0 end) Keep16_20 
  ,sum(case when BoardCount>20 and BoardCount<26 then UsersCount else 0 end) Board21_25 ,sum(case when BoardCount>20 and BoardCount<26 then UserKeep else 0 end) Keep21_25 
  ,sum(case when BoardCount>25 and BoardCount<31 then UsersCount else 0 end) Board26_30 ,sum(case when BoardCount>25 and BoardCount<31 then UserKeep else 0 end) Keep26_30 
  ,sum(case when BoardCount>30 and BoardCount<36 then UsersCount else 0 end) Board31_35 ,sum(case when BoardCount>30 and BoardCount<36 then UserKeep else 0 end) Keep31_35 
  ,sum(case when BoardCount>35 and BoardCount<41 then UsersCount else 0 end) Board36_40 ,sum(case when BoardCount>35 and BoardCount<41 then UserKeep else 0 end) Keep36_40 
  ,sum(case when BoardCount>40 and BoardCount<46 then UsersCount else 0 end) Board41_45 ,sum(case when BoardCount>40 and BoardCount<46 then UserKeep else 0 end) Keep41_45 
  ,sum(case when BoardCount>45 and BoardCount<51 then UsersCount else 0 end) Board46_50 ,sum(case when BoardCount>45 and BoardCount<51 then UserKeep else 0 end) Keep46_50
  ,sum(case when BoardCount>50 then UsersCount else 0 end) BoardMore50 ,sum(case when BoardCount>50 then UsersCount else 0 end) KeepMore50  
from record.Clearing_GameKeep a 
where a.CountDate >= '{2}' and CountDate < '{3}' and UserType = {0} and (GameType = {1} or GameType = 0) 
  and Agent = case when {4} = 0 then Agent else {4} end group by CountDate
)b on CountDate = date_add('{2}' ,interval o.id day)
where o.id < datediff('{3}' ,'{2}') order by 1 desc ;

",
model.UserType,
model.Gametype,
model.StartDate,
model.ExpirationDate,
model.Channels);


            PagedList<BoardDist> obj = new PagedList<BoardDist>(DAL.PagedListDAL<BoardDist>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }


        public static IEnumerable<BoardDetail_Left> GetBoardDetail_LeftList(GameRecordView model) {
            return DAL.GameDataDAL.GetBoardDetail_LeftList(model);
        }

        public static IEnumerable<BoardDetail_Right> GetBoardDetail_RightList(GameRecordView model)
        {
            return DAL.GameDataDAL.GetBoardDetail_RightList(model);
        }


        public static PagedList<DownLevel> GetListByPageForDownLevel(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = 1;
            pq.PageSize = 1000000;

            //  pq.RecordCount = DAL.PagedListDAL<BoardDist>.GetRecordCount(string.Format(@"select count(0) from record.BG_TexProPot where CreateTime between '{0}' and '{1}'  {2} ", model.StartDate, model.ExpirationDate, model.SearchExt == "" ? " " : " and  BoardNo = '" + model.SearchExt + "'"), sqlconnectionStringForRecord);
            pq.RecordCount = 1000000;
            pq.Sql = string.Format(@"

select date_add('{0}' ,interval o.id day) CreateTime 
  ,ifnull(Board0,0) PLess500
  ,ifnull(Board1,0) P500_1K
  ,ifnull(Board2,0) P1K_2K
  ,ifnull(Board3,0) P2K_5K
  ,ifnull(Board4,0) P5K_1W
  ,ifnull(Board5,0) P1W_5W
  ,ifnull(Board6,0) P5W_10W
  ,ifnull(Board7,0) P10W_25W
  ,ifnull(Board8,0) P25W_50W
  ,ifnull(Board9,0) P50W_100W
  ,ifnull(Board10,0) P100W_500W
  ,ifnull(Board11,0) P500W_2000W
  ,ifnull(Board12,0) PMore2000W
from record.S_Ordinal o left join(
select CountDate
  ,sum(case when BetType =1 then BetCount else 0 end) Board0 
  ,sum(case when BetType = 2 then BetCount else 0 end) Board1 
  ,sum(case when BetType = 3 then BetCount else 0 end) Board2 
  ,sum(case when BetType = 4 then BetCount else 0 end) Board3 
  ,sum(case when BetType =5 then BetCount else 0 end) Board4 
  ,sum(case when BetType =6 then BetCount else 0 end) Board5 
  ,sum(case when BetType=7 then  BetCount else 0 end) Board6  
  ,sum(case when BetType=8 then BetCount else 0 end) Board7 
  ,sum(case when BetType=9  then BetCount else 0 end) Board8 
  ,sum(case when BetType=10  then BetCount else 0 end) Board9 
  ,sum(case when BetType=11  then BetCount else 0 end) Board10 
  ,sum(case when BetType=12 then BetCount else 0 end) Board11 
  ,sum(case when BetType=13 then BetCount else 0 end) Board12 
from record.Clearing_Bet a 
where a.CountDate >= '{0}' and a.CountDate < '{1}' and 
a.UserType = {2} and GameType in ( {3} ) 
  and Agent = case when {4} = 0 then Agent else {4} end group by CountDate
)b on CountDate = date_add('{0}' ,interval o.id day)
where o.id < datediff('{1}' ,'{0}') order by 1 desc ;
",
model.StartDate,
model.ExpirationDate,
model.UserType,
model.GametypeS,
model.Channels,
model.SearchExt == "" ? " " : " and  BoardNo = '" + model.SearchExt + "'");
            PagedList<DownLevel> obj = new PagedList<DownLevel>(DAL.PagedListDAL<DownLevel>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }








        public static PagedList<LandGameRecord> GetListByPageForLand(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

            if (model.UserID > 0)
            {

                pq.RecordCount = DAL.PagedListDAL<LandGameRecord>.GetRecordCount(string.Format(@"select count(0) from BG_LandGameRecord where CreateTime between '{0}' and '{1}' and UserID like '%{2}_%'", model.StartDate, model.ExpirationDate, model.UserID), sqlconnectionStringForRecord);
                pq.Sql = string.Format(@"select * from BG_LandGameRecord where CreateTime between '{2}' and '{3}' and CONCAT('_',UserID) like '%\_{4}\_%'  order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.UserID);
            }
            else
            {

                pq.RecordCount = DAL.PagedListDAL<LandGameRecord>.GetRecordCount(string.Format(@"select count(0) from BG_LandGameRecord where CreateTime between '{0}' and '{1}'", model.StartDate, model.ExpirationDate), sqlconnectionStringForRecord);
                pq.Sql = string.Format(@"select * from BG_LandGameRecord where CreateTime between '{2}' and '{3}' order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);
            }


            PagedList<LandGameRecord> obj = new PagedList<LandGameRecord>(DAL.PagedListDAL<LandGameRecord>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }


        public static PagedList<ScaleGameRecord> GetListByPageForScale(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;


            if (Convert.ToInt64(model.Data) > 0)
            {

                pq.RecordCount = DAL.PagedListDAL<ScaleGameRecord>.GetRecordCount(string.Format(@"select count(0) from BG_ScaleGameRecord where CreateTime between '{0}' and '{1}' and Round = {2}", model.StartDate, model.ExpirationDate, model.Data), sqlconnectionStringForRecord);
                //  pq.Sql = string.Format(@"select BG_ScaleGameRecord.*,Role.Account from BG_ScaleGameRecord,515game.Role where BG_ScaleGameRecord.CreateTime between '{2}' and '{3}' and Round = {4} and BG_ScaleGameRecord.UserID =515game.Role.ID order by BG_TexasGameRecord.CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Data);

                pq.Sql = string.Format(@"select * from BG_ScaleGameRecord where CreateTime between '{2}' and '{3}' and Round = {4} order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Data);

//                pq.Sql = string.Format(@"select f.*, 515game.Role.Account from (
//select *, LEFT(UserData, INSTR(UserData, ',') - 1) as UserID
//from BG_ScaleGameRecord where CreateTime between '{2}' and '{3}' and Round = {4} order by CreateTime desc limit {0}, {1}
//) f,515game.Role
//where f.UserID = 515game.Role.ID",  pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Data);


            }
            else
            {
                pq.RecordCount = DAL.PagedListDAL<ScaleGameRecord>.GetRecordCount(string.Format(@"select count(0) from BG_ScaleGameRecord where CreateTime between '{0}' and '{1}'", model.StartDate, model.ExpirationDate), sqlconnectionStringForRecord);
                //pq.Sql = string.Format(@"select BG_ScaleGameRecord.*,Role.Account from BG_ScaleGameRecord,515game.Role where BG_ScaleGameRecord.CreateTime between '{2}' and '{3}' and BG_ScaleGameRecord.UserID =515game.Role.ID order by BG_ScaleGameRecord.CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);
                // pq.Sql = string.Format(@"select * from BG_ScaleGameRecord where CreateTime between '{2}' and '{3}'  order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);

                pq.Sql = string.Format(@"select * from BG_ScaleGameRecord where CreateTime between '{2}' and '{3}' order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);


//                pq.Sql = string.Format(@"select f.*, 515game.Role.Account from (
//select *, LEFT(UserData, INSTR(UserData, ',') - 1) as UserID
//from BG_ScaleGameRecord where CreateTime between '{2}' and '{3}'  order by CreateTime desc limit {0}, {1}
//) f,515game.Role
//where f.UserID = 515game.Role.ID", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);

            }



            PagedList<ScaleGameRecord> obj = new PagedList<ScaleGameRecord>(DAL.PagedListDAL<ScaleGameRecord>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }

        /// <summary>
        /// 百人德州
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static PagedList<ScaleGameRecord> GetListByPageForTexPro(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;


            if (Convert.ToInt64(model.Data) > 0)
            {

                pq.RecordCount = DAL.PagedListDAL<ScaleGameRecord>.GetRecordCount(string.Format(@"select count(0) from BG_TexProGameRecord where CreateTime between '{0}' and '{1}' and Round = {2}", model.StartDate, model.ExpirationDate, model.Data), sqlconnectionStringForRecord);
                //  pq.Sql = string.Format(@"select BG_ScaleGameRecord.*,Role.Account from BG_ScaleGameRecord,515game.Role where BG_ScaleGameRecord.CreateTime between '{2}' and '{3}' and Round = {4} and BG_ScaleGameRecord.UserID =515game.Role.ID order by BG_TexasGameRecord.CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Data);

                pq.Sql = string.Format(@"select * from BG_TexProGameRecord where CreateTime between '{2}' and '{3}' and Round = {4} order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Data);

                //                pq.Sql = string.Format(@"select f.*, 515game.Role.Account from (
                //select *, LEFT(UserData, INSTR(UserData, ',') - 1) as UserID
                //from BG_ScaleGameRecord where CreateTime between '{2}' and '{3}' and Round = {4} order by CreateTime desc limit {0}, {1}
                //) f,515game.Role
                //where f.UserID = 515game.Role.ID",  pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Data);


            }
            else
            {
                pq.RecordCount = DAL.PagedListDAL<ScaleGameRecord>.GetRecordCount(string.Format(@"select count(0) from BG_TexProGameRecord where CreateTime between '{0}' and '{1}'", model.StartDate, model.ExpirationDate), sqlconnectionStringForRecord);
                //pq.Sql = string.Format(@"select BG_ScaleGameRecord.*,Role.Account from BG_ScaleGameRecord,515game.Role where BG_ScaleGameRecord.CreateTime between '{2}' and '{3}' and BG_ScaleGameRecord.UserID =515game.Role.ID order by BG_ScaleGameRecord.CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);
                // pq.Sql = string.Format(@"select * from BG_ScaleGameRecord where CreateTime between '{2}' and '{3}'  order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);

                pq.Sql = string.Format(@"select * from BG_TexProGameRecord where CreateTime between '{2}' and '{3}' order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);


                //                pq.Sql = string.Format(@"select f.*, 515game.Role.Account from (
                //select *, LEFT(UserData, INSTR(UserData, ',') - 1) as UserID
                //from BG_ScaleGameRecord where CreateTime between '{2}' and '{3}'  order by CreateTime desc limit {0}, {1}
                //) f,515game.Role
                //where f.UserID = 515game.Role.ID", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);

            }



            PagedList<ScaleGameRecord> obj = new PagedList<ScaleGameRecord>(DAL.PagedListDAL<ScaleGameRecord>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }

        public static PagedList<TexasGameRecord> GetListByPageForTexas(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

            if (!string.IsNullOrEmpty(model.SearchExt))
            {

                //pq.RecordCount = DAL.PagedListDAL<TexasGameRecord>.GetRecordCount(string.Format(@"select count(0) from BG_TexasGameRecord where CreateTime between '{0}' and '{1}' and  CONCAT('_',BG_TexasGameRecord.UserID) like '%\_{2}\_%' ", model.StartDate, model.ExpirationDate, model.UserID), sqlconnectionStringForRecord);

                pq.RecordCount = DAL.PagedListDAL<TexasGameRecord>.GetRecordCount(string.Format(@"  select count(0) from record.BG_TexasGameRecord as b,(select ID  from 515game.Role where ID='{2}' or  Account='{2}' or NickName='{2}') as r
            where b.CreateTime between '{0}' and '{1}' and LOCATE(CONCAT('_',r.ID,'_'), CONCAT('_',b.UserID) )!=0
", model.StartDate, model.ExpirationDate, model.SearchExt), sqlconnectionStringForRecord);
                //pq.Sql = string.Format(@"select BG_TexasGameRecord.*,Role.Account from BG_TexasGameRecord,515game.Role where BG_TexasGameRecord.CreateTime between '{2}' and '{3}' and BG_TexasGameRecord.UserID like '%{4}_%' and BG_TexasGameRecord.UserID =515game.Role.ID order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.UserID);
                // pq.Sql = string.Format(@"select * from BG_TexasGameRecord where CreateTime between '{2}' and '{3}' and UserID like '%{4}_%' order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.UserID);
                pq.Sql = string.Format(@"
            select b.* from record.BG_TexasGameRecord as b,(select ID  from 515game.Role where ID='{4}' or  Account='{4}' or NickName='{4}') as r
            where b.CreateTime between '{2}' and '{3}' and LOCATE(CONCAT('_',r.ID,'_'), CONCAT('_',b.UserID) )!=0
            order by b.CreateTime desc 
            limit {0}, {1};
   ", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.SearchExt);
                /*
            select b.* from record.BG_TexasGameRecord as b,(select ID  from Role where ID='ccc13393' or  Account='ccc13393' or NickName='ccc13393') as r
            where b.CreateTime between '2015-9-16' and '2015-12-18' and LOCATE(CONCAT('_',r.ID,'_'), CONCAT('_',b.UserID) )!=0
            order by b.CreateTime desc 
            limit 0, 10;
    
            
            
            
            */




                //pq.Sql = string.Format(@"select BG_TexasGameRecord.*,Role.Account from BG_TexasGameRecord,515game.Role where BG_TexasGameRecord.CreateTime between '{2}' and '{3}' and  CONCAT('_',BG_TexasGameRecord.UserID) like '%\_{4}\_%'  and BG_TexasGameRecord.UserID =515game.Role.ID order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.UserID);
            }
            else
            {

                pq.RecordCount = DAL.PagedListDAL<TexasGameRecord>.GetRecordCount(string.Format(@"select count(0) from BG_TexasGameRecord where CreateTime between '{0}' and '{1}'", model.StartDate, model.ExpirationDate), sqlconnectionStringForRecord);
              //  pq.Sql = string.Format(@"select BG_TexasGameRecord.*,Role.Account from BG_TexasGameRecord,515game.Role where BG_TexasGameRecord.CreateTime between '{2}' and '{3}'  and BG_TexasGameRecord.UserID =515game.Role.ID order by BG_TexasGameRecord.CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);


                pq.Sql = string.Format(@"select * from BG_TexasGameRecord where CreateTime between '{2}' and '{3}' order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);
            }

            PagedList<TexasGameRecord> obj = new PagedList<TexasGameRecord>(DAL.PagedListDAL<TexasGameRecord>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }


        public static PagedList<DotSum> GetListByPageForDotSumByModuleID(GameRecordView model, int modelID ) {


            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

            //时间范围 * 平台数
            pq.RecordCount = DAL.PagedListDAL<DotSum>.GetRecordCount(string.Format(@"
                select count(0) * (case when {2} = 0 then 3 else 1 end) from S_Ordinal where id < datediff('{1}' ,'{0}'  )"
            , model.StartDate, model.ExpirationDate, model.Channels), sqlconnectionStringForRecord);


            pq.Sql = string.Format(@"
                select date_add('{2}' ,interval a.id day) CreateTime ,c.TypeID ClientType ,ifnull(sum(Active) ,0) Active ,ifnull(sum(ClickNum) ,0) ClickNum ,ifnull(sum(ClickCount) ,0) ClickCount 
                from S_Ordinal a 
                join (select distinct TypeID from GServerInfo.AgentUserRoles where TypeID = case when {4}=0 then TypeID else {4} end) c on 1 = 1
                left join ( 
                  select a.PlatType ClientType ,0 Active ,count(distinct  UserID ) ClickNum 
                    ,sum(ClickCount) ClickCount ,a.CreateTime 
                  from BG_ClickRecord a 
                    join 515game.Role b on a.UserID = b.ID and b.isfreeze = 0 
                  where a.ModeluID = {5} and a.CreateTime >= '{2}' and a.CreateTime < '{3}' and a.PlatType = case when {4}=0 then a.PlatType else {4} end 
                    and a.SiteID in (1,2) group by a.CreateTime ,a.PlatType
                union all
                  select a.TypeID ,sum(SumValue) Active ,0  ,0 ,b.CountDate
                  from (select UserId ,TypeID from GServerInfo.AgentUserRoles where TypeID = case when {4}=0 then TypeID else {4} end)a
                    left join Clearing_Day b on b.Agent = a.UserId and b.TypeID = 103 and b.CountDate >= '{2}' and b.CountDate < '{3}' 
                  group by b.CountDate ,a.TypeID 
                )b on b.CreateTime = date_add('{2}' ,interval a.id day) and c.TypeID = b.ClientType 
                where a.id < datediff('{3}' ,'{2}') group by c.TypeID ,date_add('{2}' ,interval a.id day) order by CreateTime desc ,ClientType limit {0}, {1}"
            , pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Channels, modelID);


            PagedList<DotSum> obj = new PagedList<DotSum>(DAL.PagedListDAL<DotSum>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }


        public static PagedList<DotSum> GetListByPageForDotSum(GameRecordView model,int modelID=1)
        {
           // model.StartDate;开始时间
           // model.ExpirationDate;结束时间
           // model.Channels;//客户端

            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

            //时间范围 * 平台数
            pq.RecordCount = DAL.PagedListDAL<DotSum>.GetRecordCount(string.Format(@"
                select count(0) * (case when {2} = 0 then 3 else 1 end) from S_Ordinal where id < datediff('{1}' ,'{0}'  )"
            , model.StartDate, model.ExpirationDate ,model.Channels), sqlconnectionStringForRecord);


            pq.Sql = string.Format(@"
                select date_add('{2}' ,interval a.id day) CreateTime ,c.TypeID ClientType ,ifnull(sum(Active) ,0) Active ,ifnull(sum(ZFBNum) ,0) ZFBNum 
                  ,ifnull(sum(ZFBCount) ,0) ZFBCount ,ifnull(sum(ClickNum) ,0) ClickNum ,ifnull(sum(ClickCount) ,0) ClickCount 
                from S_Ordinal a 
                join (select distinct TypeID from GServerInfo.AgentUserRoles where TypeID = case when {4}=0 then TypeID else {4} end) c on 1 = 1
                left join ( 
                  select a.PlatType ClientType ,0 Active ,count(distinct case a.SiteID when 1 then UserID end) ZFBNum 
                    ,sum(case a.SiteID when 1 then ClickCount else 0 end) ZFBCount ,count(distinct case a.SiteID when 2 then UserID end) ClickNum 
                    ,sum(case a.SiteID when 2 then ClickCount else 0 end) ClickCount ,a.CreateTime 
                  from BG_ClickRecord a 
                    join 515game.Role b on a.UserID = b.ID and b.isfreeze = 0 
                  where a.ModeluID = {5} and a.CreateTime >= '{2}' and a.CreateTime < '{3}' and a.PlatType = case when {4}=0 then a.PlatType else {4} end 
                    and a.SiteID in (1,2) group by a.CreateTime ,a.PlatType
                union all
                  select a.TypeID ,sum(SumValue) Active ,0 ,0 ,0 ,0 ,b.CountDate
                  from (select UserId ,TypeID from GServerInfo.AgentUserRoles where TypeID = case when {4}=0 then TypeID else {4} end)a
                    left join Clearing_Day b on b.Agent = a.UserId and b.TypeID = 103 and b.CountDate >= '{2}' and b.CountDate < '{3}' 
                  group by b.CountDate ,a.TypeID 
                )b on b.CreateTime = date_add('{2}' ,interval a.id day) and c.TypeID = b.ClientType 
                where a.id < datediff('{3}' ,'{2}') group by c.TypeID ,date_add('{2}' ,interval a.id day) order by CreateTime desc ,ClientType limit {0}, {1}"
            , pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate ,model.Channels, modelID);
          

            PagedList<DotSum> obj = new PagedList<DotSum>(DAL.PagedListDAL<DotSum>.GetListByPage(pq, sqlconnectionStringForRecord), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }

        public static PagedList<ZodiacGameRecord> GetListByPageForZodiac(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

            if (Convert.ToInt64(model.Data) > 0)
            {

                pq.RecordCount = DAL.PagedListDAL<ZodiacGameRecord>.GetRecordCount(string.Format(@"select count(0) from record.BG_ZodiacGameRecord where CreateTime between '{0}' and '{1}' and Round like '%{2}%'", model.StartDate, model.ExpirationDate, model.Data), sqlconnectionString);
                pq.Sql = string.Format(@"select * from record.BG_ZodiacGameRecord where CreateTime between '{2}' and '{3}' and Round = {4} order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Data);


//                pq.Sql = string.Format(@"select f.*, 515game.Role.Account from (
//select *,LEFT(UserData,INSTR(UserData,',')-1) as UserID
//from 515game.BG_ZodiacGameRecord  where CreateTime between '{2}' and '{3}'  and Round = {4} order by CreateTime desc limit {0}, {1}
//) f,515game.Role
//where f.UserID = 515game.Role.ID;

//", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Data);


            }
            else
            {

                pq.RecordCount = DAL.PagedListDAL<ZodiacGameRecord>.GetRecordCount(string.Format(@"select count(0) from record.BG_ZodiacGameRecord where CreateTime between '{0}' and '{1}'", model.StartDate, model.ExpirationDate), sqlconnectionString);
                pq.Sql = string.Format(@"select * from record.BG_ZodiacGameRecord where CreateTime between '{2}' and '{3}' order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);


//                pq.Sql = string.Format(@"select f.*, 515game.Role.Account from (
//select *,LEFT(UserData,INSTR(UserData,',')-1) as UserID
//from 515game.BG_ZodiacGameRecord  where CreateTime between '{2}' and '{3}'  order by CreateTime desc limit {0}, {1}
//) f,515game.Role
//where f.UserID = 515game.Role.ID;

//",pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);
            }

            PagedList<ZodiacGameRecord> obj = new PagedList<ZodiacGameRecord>(DAL.PagedListDAL<ZodiacGameRecord>.GetListByPage(pq, sqlconnectionString), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }


        public static PagedList<HorseGameRecord> GetListByPageForHorse(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

            if (Convert.ToInt64(model.Data) > 0)
            {

                pq.RecordCount = DAL.PagedListDAL<HorseGameRecord>.GetRecordCount(string.Format(@"select count(0) from record.BG_HorseGameRecord where CreateTime between '{0}' and '{1}' and RoundID = '{2}'", model.StartDate, model.ExpirationDate, model.Data), sqlconnectionString);
                pq.Sql = string.Format(@"select * from record.BG_HorseGameRecord where CreateTime between '{2}' and '{3}' and RoundID = {4} order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Data);


                //                pq.Sql = string.Format(@"select f.*, 515game.Role.Account from (
                //select *,LEFT(UserData,INSTR(UserData,',')-1) as UserID
                //from 515game.BG_ZodiacGameRecord  where CreateTime between '{2}' and '{3}'  and Round = {4} order by CreateTime desc limit {0}, {1}
                //) f,515game.Role
                //where f.UserID = 515game.Role.ID;

                //", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Data);


            }
            else
            {

                pq.RecordCount = DAL.PagedListDAL<HorseGameRecord>.GetRecordCount(string.Format(@"select count(0) from record.BG_HorseGameRecord where CreateTime between '{0}' and '{1}'", model.StartDate, model.ExpirationDate), sqlconnectionString);
                pq.Sql = string.Format(@"select * from record.BG_HorseGameRecord where CreateTime between '{2}' and '{3}' order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);


                //                pq.Sql = string.Format(@"select f.*, 515game.Role.Account from (
                //select *,LEFT(UserData,INSTR(UserData,',')-1) as UserID
                //from 515game.BG_ZodiacGameRecord  where CreateTime between '{2}' and '{3}'  order by CreateTime desc limit {0}, {1}
                //) f,515game.Role
                //where f.UserID = 515game.Role.ID;

                //",pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);
            }

            PagedList<HorseGameRecord> obj = new PagedList<HorseGameRecord>(DAL.PagedListDAL<HorseGameRecord>.GetListByPage(pq, sqlconnectionString), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }



     

        public static PagedList<CarGameRecord> GetListByPageForCar(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

            if (Convert.ToInt64(model.Data) > 0)
            {

                pq.RecordCount = DAL.PagedListDAL<CarGameRecord>.GetRecordCount(string.Format(@"select count(0) from record.BG_CarGameRecord where CreateTime between '{0}' and '{1}' and RoundID = '{2}'", model.StartDate, model.ExpirationDate, model.Data), sqlconnectionString);
                pq.Sql = string.Format(@"select * from record.BG_CarGameRecord where CreateTime between '{2}' and '{3}' and RoundID = {4} order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Data);

            }
            else
            {

                pq.RecordCount = DAL.PagedListDAL<CarGameRecord>.GetRecordCount(string.Format(@"select count(0) from record.BG_CarGameRecord where CreateTime between '{0}' and '{1}'", model.StartDate, model.ExpirationDate), sqlconnectionString);
                pq.Sql = string.Format(@"select * from record.BG_CarGameRecord where CreateTime between '{2}' and '{3}' order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);

            }

            PagedList<CarGameRecord> obj = new PagedList<CarGameRecord>(DAL.PagedListDAL<CarGameRecord>.GetListByPage(pq, sqlconnectionString), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }


        /// <summary>
        /// 龙虎斗
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static PagedList<LongHuGameRecord> GetListByPageForLongHu(GameRecordView model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

            if (Convert.ToInt64(model.Data) > 0)
            {

                pq.RecordCount = DAL.PagedListDAL<LongHuGameRecord>.GetRecordCount(string.Format(@"select count(0) from record.BG_CarGameRecord where CreateTime between '{0}' and '{1}' and RoundID = '{2}'", model.StartDate, model.ExpirationDate, model.Data), sqlconnectionString);
                pq.Sql = string.Format(@"select * from record.BG_CarGameRecord where CreateTime between '{2}' and '{3}' and RoundID = {4} order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate, model.Data);

            }
            else
            {

                pq.RecordCount = DAL.PagedListDAL<LongHuGameRecord>.GetRecordCount(string.Format(@"select count(0) from record.BG_CarGameRecord where CreateTime between '{0}' and '{1}'", model.StartDate, model.ExpirationDate), sqlconnectionString);
                pq.Sql = string.Format(@"select * from record.BG_CarGameRecord where CreateTime between '{2}' and '{3}' order by CreateTime desc limit {0}, {1}", pq.StartRowNumber, pq.PageSize, model.StartDate, model.ExpirationDate);

            }

            PagedList<LongHuGameRecord> obj = new PagedList<LongHuGameRecord>(DAL.PagedListDAL<LongHuGameRecord>.GetListByPage(pq, sqlconnectionString), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }

        public static bool Add(string sql) {
           return GameDataDAL.Add(sql)>0;
        }


        public static IEnumerable<TexasGameRecord> GetListForTexas(GameRecordView vbd) {



            return GameDataDAL.GetListForTexas(vbd);
        }
        public static TexasGameRecord GetTexasModelForRound(decimal round) {
            return GameDataDAL.GetTexasModelForRound(round);
        }

        public static IEnumerable<ScaleGameRecord> GetListForScale(GameRecordView vbd)
        {
            return GameDataDAL.GetListForScale(vbd);
        }

        public static IEnumerable<ScaleGameRecord> GetListForTexPro(GameRecordView vbd)
        {
            return GameDataDAL.GetListForTexPro(vbd);
        }


        public static IEnumerable<ZodiacGameRecord> GetListForZodiac(GameRecordView vbd)
        {
            return GameDataDAL.GetListForZodiac(vbd);
        }
        public static IEnumerable<HorseGameRecord> GetListForHorse(GameRecordView vbd)
        {
            return GameDataDAL.GetListForHorse(vbd);
        }

        public static IEnumerable<CarGameRecord> GetListForCar(GameRecordView vbd)
        {
            return GameDataDAL.GetListForCar(vbd);
        }

        public static string GetBeginTimeForGame(GameRecordView vbd) {
            return GameDataDAL.GetBeginTimeForGame(vbd);
        }
        public static DateTime GetDataBaseTime()
        {
            return GameDataDAL.GetDataBaseTime();
        }

        public static bool UpdateBeginTimeForGame(GameRecordView vbd)
        {
            return GameDataDAL.UpdateBeginTimeForGame(vbd)>0;
        }





        public static int GetSwitchIsOpen(int id)
        {
            return GameDataDAL.GetSwitchIsOpen(id);
        }

        public static SSwitch GetSwitchModel(int id)
        {
            return GameDataDAL.GetSwitchModel(id);
        }

        

        public static int UpdateSwitchIsOpen(int id,bool IsOpen)
        {
            if (IsOpen)
            {
                return GameDataDAL.UpdateSwitchIsOpen(id,1 );
            }
            else
            {
                return GameDataDAL.UpdateSwitchIsOpen(id, 0);
            }
           
        }


        public static int AddPopUpData(PopUpInfo model) {
            return GameDataDAL.AddPopUpData(model);
        }

        public static int DeletePopUpData(PopUpInfo model)
        {
            return GameDataDAL.DeletePopUpData(model);
        }

        public static int UpdatePopUpData(PopUpInfo model,int id) {
            return GameDataDAL.UpdatePopUpData(model,id);
        }


        public static IEnumerable<PopUpInfo> GetPopUpDataList()
        {
            return GameDataDAL.GetPopUpDataList();
        }


        public static PopUpInfo GetPopUpData(int id)
        {
            return GameDataDAL.GetPopUpData(id);
        }


        public static PopUpInfo GetPopUpDataByPosition(int position)
        {
            return GameDataDAL.GetPopUpDataByPosition(position);
        }




        /// <summary>
        /// 后台登录注册开关记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static PagedList<RLFlow> GetListByPageForRegisterLogin(RLFlow model)
        {
            PagerQuery pq = new PagerQuery();
            pq.CurrentPage = model.Page;
            pq.PageSize = 10;

          
            pq.RecordCount = DAL.PagedListDAL<RLFlow>.GetRecordCount(string.Format(@"
select COUNT(*) from 515game.LoginRegisteRecord
where 1=1  {0} {1} {2} {3}
", 
model.Agent>0 ?"": " and Agent = 0",
model.PhoneBoardID > 0 ? "" : "  and PhoneBoardID =0 ",
model.PhoneBoardID > 0 ? "" : "  and PhoneModelsID=0",
model.Platform > 0 ? "" : "  and Platform=0 "

), sqlconnectionString);

            pq.Sql = string.Format(@"
select * from 515game.LoginRegisteRecord
where  1=1  {0} {1} {2} {3}
order by CreateTime desc 
limit {4}, {5}",
model.Agent > 0 ? "" : " and Agent = 0",
model.PhoneBoardID > 0 ? "" : "  and PhoneBoardID =0 ",
model.PhoneBoardID > 0 ? "" : "  and PhoneModelsID=0",
model.Platform > 0 ? "" : "  and Platform=0 ",
pq.StartRowNumber, 
pq.PageSize);

         

            PagedList<RLFlow> obj = new PagedList<RLFlow>(DAL.PagedListDAL<RLFlow>.GetListByPage(pq, sqlconnectionString), pq.CurrentPage, pq.PageSize, pq.RecordCount);
            return obj;
        }




    }
}