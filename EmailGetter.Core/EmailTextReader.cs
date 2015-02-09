using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows;
using HtmlAgilityPack;
using EmailGetter.Core.Data.Model;

namespace EmailGetter.Core
{
    public static class EmailTextReader
    {
        private const string FirstNameConst = "First Name:";
        private const string LastNameConst = "Last Name:";
        private const string CompanyEmail = "Company Mailling Address:";
        private const string City = "City:";
        private const string State = "State:";
        private const string Zip = "Zip:";
        private const string Country = "Country:";
        private const string Email = "Email:";
        private const string Phone = "Phone:";
        private const string CompanyName = "Company Name:";
        private const string CompanyType = "Company Type:";
        private const string PositionTitle = "Position/ Title:";
        private const string PrimaryJobFunction = "Primary Job Function:";
        private const string CommentOrQuestion = "Comment or Question:";
        private const string EmailFooter = "This mail is sent via contact form on Virtium Technology http://www.virtium.com";

        public static ContactForm ReadContactFrom(string text)
        {
            ContactForm contact = new ContactForm();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(text);

            StringBuilder sb = new StringBuilder();
            foreach (HtmlTextNode node in doc.DocumentNode.SelectNodes("//text()"))
            {
                sb.AppendLine(node.Text);
            }

            string final = sb.ToString();
            string[] lines = final.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            int i = 0;
            foreach(var line in lines)
            {
                if (line.Contains(FirstNameConst))
                {
                    if (lines[i + 1] != LastNameConst)
                        contact.FirstName = lines[i + 1];
                }
                else if (line.Contains(LastNameConst))
                {
                    if (lines[i + 1] != CompanyEmail)
                        contact.LastName = lines[i + 1];
                }
                else if (line.Contains(CompanyEmail))
                {
                    if (lines[i + 1] != City)
                        contact.CompanyEmail = lines[i + 1];
                }
                else if (line.Contains(City))
                {
                    if (lines[i + 1] != State)
                        contact.City = lines[i + 1];
                }
                else if (line.Contains(State))
                {
                    if (lines[i + 1] != Zip)
                        contact.State = lines[i + 1];
                    //I know this bad, but we have some column is not standard, we must to hard code at the time, will optimize later
                    if (lines[i + 1] == "&nbsp;")
                        contact.State = lines[i + 2];
                }
                else if (line.Contains(Zip))
                {
                    if (lines[i + 1] != Country)
                        contact.Zip = lines[i + 1];
                }
                else if (line.Contains(Country))
                {
                    if (lines[i + 1] != Email)
                        contact.Country = lines[i + 1];
                }
                else if (line.Contains(Email))
                {
                    if (lines[i + 1] != Phone)
                        contact.Email = lines[i + 1];
                }
                else if (line.Contains(Phone))
                {
                    if (lines[i + 1] != CompanyName)
                        contact.Phone = lines[i + 1];
                }
                else if (line.Contains(CompanyName))
                {
                    if (lines[i + 1] != CompanyType)
                        contact.CompanyName = lines[i + 1];
                }
                else if (line.Contains(CompanyType))
                {
                    if (lines[i + 1] != PositionTitle)
                        contact.CompanyType = lines[i + 1];
                }
                else if (line.Contains(PositionTitle))
                {
                    if (lines[i + 1] != PrimaryJobFunction)
                        contact.PositionTitle = lines[i + 1];
                    //I know this bad, but we have some column is not standard, we must to hard code at the time, will optimize later
                    if (lines[i + 1] == "&nbsp;")
                        contact.PositionTitle = lines[i + 2];
                }
                else if (line.Contains(PrimaryJobFunction))
                {
                    if (lines[i + 1] != CommentOrQuestion)
                        contact.PrimaryJobFunction = lines[i + 1];
                    //I know this bad, but we have some column is not standard, we must to hard code at the time, will optimize later
                    if (lines[i + 1] == "&nbsp;")
                        contact.PrimaryJobFunction = lines[i + 2];
                }
                else if (line.Contains(CommentOrQuestion))
                {
                    if (lines[i + 1] != EmailFooter)
                        contact.CommentOrQuestion = lines[i + 1];
                    //I know this bad, but we have some column is not standard, we must to hard code at the time, will optimize later
                    if (lines[i + 1] == "&nbsp;")
                        contact.CommentOrQuestion = lines[i + 2];
                }
                i++;
            }
            return contact;
        }
    }
}
