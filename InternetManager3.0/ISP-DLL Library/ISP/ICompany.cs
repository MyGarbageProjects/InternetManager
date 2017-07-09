using System.Collections.Generic;
namespace ISP
{
    interface ICompany
    {
        double SpentMoney { get; }
        double Balance { get; }
        string Name { get; }
        void Auth(string Login, string Password, out List<string> fields);
        void GetValue(string html, ref List<string> Items);
    }
}
