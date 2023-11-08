using PhanQuyenAPI.Models;
using PhanQuyenAPI.Constants;
namespace PhanQuyenAPI.IServices {
    interface IPhanQuyenAPIServices {
        ErorrType Register(string userName,string passWord,string confirmPassword);
        ErorrType Login(Accounts acc);
        ErorrType ChangePassWord(string userName,string passWord, string newPass);
        ErorrType ChangeStatus(string userName, string newStatus);
        ErorrType BanAccount(string userName);
        ErorrType ForgotPassword(string email);
        List<Accounts> DSAccounts();
    }
}
