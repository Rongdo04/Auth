namespace PhanQuyenAPI.Models {
    public class Decentralizations {
        public int DecentralizationsID { get; set; }
        public string AuthorityName { get; set; }// Tên quyền
        public DateTime createAt { get; set; }//Ngày tạo
        public DateTime updateAt { get; set; }//Ngày cập nhật
        List<Accounts>? Accounts { get; set; }
       
    }
}
