

namespace PhanQuyenAPI.Models {
    public class Accounts {
        public int AccountsID { get; set; }
        
        public string userName { get; set; }// tên người dùng
        public string? avatar { get; set; }// avatar
        public string? email { get; set; }
        public string password { get; set; }// mật khẩu đã mã hóa
        public string? status { get; set; }// trạng thái tài khoản
        public int DecentralizationsID { get; set; }
        public Decentralizations? decentralizations { get; set; }
        public string? ResetPasswordToken { get; set; }// Token để resetpassword
        public DateTime? ResetPasswordTokenExpiry { get; set; }// thời hạn của token
        public DateTime? createAt { get; set; }//Ngày tạo
        public DateTime? updateAt { get; set; } //Ngày cập nhật
    }
}
