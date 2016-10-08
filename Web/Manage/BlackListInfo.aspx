﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Manage/Site1.Master" AutoEventWireup="true" CodeBehind="BlackListInfo.aspx.cs" Inherits="GS.Web.Manage.BlackListInfo" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
      <link href="/css/AspNetPage.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <form id="form1" runat="server">
      <div id="content-header">
    <div id="breadcrumb"> <a href="#" title="Go to Home" class="tip-bottom"><i class="icon-home"></i> 首页</a><a href="#" class="tip-bottom">黑名单管理</a> <a href="#" class="current">黑名单信息</a> </div>
         <h1>黑名单信息</h1>
  </div>

        <div class="container-fluid">

    <hr>
    <div class="row-fluid">
     <div class="widget-box">
          <div class="widget-title"> <span class="icon"> <i class="icon-th"></i> </span>
            <h5>黑名单信息列表</h5>
          </div>

          <div class="widget-content nopadding">
  

         <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand">
             <HeaderTemplate>

                    <table class="table table-bordered table-striped">
              <thead>
                <tr>
                  <th>真实姓名</th>
                  <th>帐号</th>
                  <th>手机</th>
                  <th>邮箱</th>
                    <th>QQ</th>
                    <th>登录IP</th>
                    
                    <th>银行资料</th>
                  <th>基本操作</th>
                </tr>
              </thead>
             </HeaderTemplate>
             
             <ItemTemplate>
                    <tbody>
                <tr class="odd gradeX">
                  <td><%# Eval("RealName") %></td>
                  <td><%# Eval("Account") %></td>
                  <td><%# Eval("MobilePhone") %></td>
                    <td><%# Eval("Email") %></td>

                        <td><%# Eval("QQ") %></td>

                        <td><%# Eval("LoginIP") %></td>
                     <td><%# Eval("BankInformation") %></td>

                  <td style="text-align:center"> <asp:LinkButton ID="linkdel" CommandArgument='<%# Eval("bId") %>'  OnClientClick="return confirm('确定删除？')" CommandName="Del" CssClass="btn btn-danger btn-mini" runat="server"  Text="删除"></asp:LinkButton>

                  </td>
                 
                </tr>
              
              </tbody>
          

             </ItemTemplate>
             <FooterTemplate>

                            
         <tbody>                <tr >
                     <td  colspan="8" style="text-align:center;">

  <asp:Label ID="lblEmpty"     
     
Text="暂无数据!" Font-Bold="true" ForeColor="Red"  Font-Size="Large" runat="server"     
     
Visible='<%# bool.Parse((Repeater1.Items.Count==0).ToString())%>'>      
     
</asp:Label> 
                     </td>


                 </tr>
                 </tbody>
                   </table>
             </FooterTemplate>

         </asp:Repeater>
           
          </div>
         
    <webdiyer:AspNetPager ID="AspNetPager1"  CssClass="paginator" CurrentPageButtonClass="cpb" runat="server" PageSize="10" OnPageChanged="AspNetPager1_PageChanged"   FirstPageText="首页" LastPageText="尾页" NextPageText="下一页" PrevPageText="上一页"></webdiyer:AspNetPager>
        </div>
        </div></div>
        </form>
</asp:Content>