using PhanQuyenAPI.Constants;
using PhanQuyenAPI.IServices;
using PhanQuyenAPI.Models;
using Microsoft.AspNetCore.Identity;
namespace PhanQuyenAPI.Services {
    public class PhanQuyenAPIServices : IPhanQuyenAPIServices {
        private readonly AppDbContext dbContext;
       
        public PhanQuyenAPIServices() {
            dbContext = new AppDbContext();
        }
        
        public ErorrType BanAccount(string userName) {
            // Tìm tài khoản trong cơ sở dữ liệu dựa trên userName
            var account = dbContext.Account.FirstOrDefault(x => x.userName == userName);

            if (account == null) {
                // Nếu tài khoản không tồn tại, trả về lỗi TaiKhoanKhongTonTai
                return ErorrType.TaiKhoanKhongTonTai;
            }

            // Kiểm tra trạng thái của tài khoản
            if (account.status != null) {
                if (account.status.ToLower() == "banned") {
                    // Nếu trạng thái là "banned," thay đổi thành "inactive" và lưu lại
                    account.status = "inactive";
                    dbContext.SaveChanges();
                    return ErorrType.ThanhCong;
                } else  {
                    // Nếu trạng thái là "inactive," trả về lỗi TaiKhoanBiBan
                    account.status = "Banned";
                    dbContext.SaveChanges();

                    return ErorrType.TaiKhoanBiBan;
                }
            }

            // Kiểm tra vai trò của người dùng hoặc quản trị viên
            var userOrAdmin = dbContext.Decentralization.FirstOrDefault(x => x.DecentralizationsID == account.DecentralizationsID);

            if (userOrAdmin != null) {
                if (userOrAdmin.AuthorityName.ToLower() == "admin") {
                    // Nếu tài khoản thuộc vai trò quản trị viên, trả về lỗi ThatBai
                    return ErorrType.ThatBai;
                }
            }

            // Nếu không có trạng thái hoặc không phải quản trị viên, thay đổi trạng thái thành "Banned" và lưu lại
            account.status = "Banned";
            dbContext.SaveChanges();
            return ErorrType.ThanhCong;
        }

        

        public ErorrType ChangeStatus(string userName,string newStatus) {
            var account = dbContext.Account.FirstOrDefault(x => x.userName == userName);
            if (account != null) {
                account.status = newStatus;
                dbContext.SaveChanges();
                return ErorrType.ThanhCong;
            }
            else {
                return ErorrType.ThatBai;
            }

        }

        public List<Accounts> DSAccounts() {
            var accounts = dbContext.Account.ToList();
            return accounts;
        }

        public ErorrType Login(Accounts acc) {
            if (string.IsNullOrEmpty(acc.userName) || string.IsNullOrEmpty(acc.password)) {
                return ErorrType.KhongDuocDeTrong;
            }

            var user = dbContext.Account.FirstOrDefault(x => x.userName == acc.userName);

            if (user == null) {
                return ErorrType.ThatBai;
            }
            // Sử dụng thư viện băm mật khẩu  để so sánh mật khẩu
            try {
                if (!BCrypt.Net.BCrypt.Verify(acc.password, user.password)) {
                    return ErorrType.ThatBai;
                } else {
                    throw new Exception("");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }


            var userRoles = dbContext.Decentralization.Where(ur => ur.DecentralizationsID == user.DecentralizationsID)
                                               .Select(ur => ur.AuthorityName.ToLower()).ToList();

           
            acc.DecentralizationsID = user.DecentralizationsID;
            if (userRoles.Contains("admin")) {
                return ErorrType.AdminLogin;
            } else if (userRoles.Contains("user")) {
                
                return ErorrType.UserLogin;
            }

            return ErorrType.ThatBai;
        }
        public ErorrType ForgotPassword(string email) {
            return ErorrType.ThatBai;
        }

        public ErorrType Register(string userName, string passWord, string confirmPassword) {
            var taiKhoanHienTai = dbContext.Account.FirstOrDefault(x => x.userName.Equals(userName));
            var roleUser = dbContext.Decentralization.FirstOrDefault(x => x.AuthorityName.ToLower().Equals("user"));
            if(passWord == confirmPassword) {
                if (taiKhoanHienTai != null) {
                    return ErorrType.TaiKhoanDaTonTai;
                } else {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(passWord);
                    Accounts acc = new Accounts() {

                        userName = userName,
                        password = hashedPassword,
                        createAt = DateTime.Now,

                    };
                    if (roleUser != null) {
                        acc.DecentralizationsID = roleUser.DecentralizationsID;
                    }
                    dbContext.Account.Add(acc);
                    dbContext.SaveChanges();
                    return ErorrType.ThanhCong;
                }
            }
            else {
                return ErorrType.ThatBai;
            }
        }

       


     

        ErorrType IPhanQuyenAPIServices.ChangePassWord(string userName, string passWord,string newPass) {
            var user = dbContext.Account.FirstOrDefault(x => x.userName == userName);
            if (user == null) {
                return ErorrType.TaiKhoanKhongTonTai;
            }
            if (BCrypt.Net.BCrypt.Verify(passWord, user.password)) {
                // Mật khẩu đã băm và trùng khớp
                user.password = BCrypt.Net.BCrypt.HashPassword(newPass); // Băm mật khẩu mới
                user.updateAt = DateTime.Now;
                dbContext.SaveChanges();
                return ErorrType.ThanhCong;
            } else {
                return ErorrType.ThatBai;
            }
        }
       
    }
}
