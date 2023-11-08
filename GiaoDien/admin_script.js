function toggleBan(userName, banButton) {
  const BanAccData = {
    username: userName,
  };

  // Gọi API để cấm/gỡ ban tài khoản
  fetch("https://localhost:7281/api/Auth/BanAcc", {
    method: "PUT", // Sử dụng phương thức PUT để gửi yêu cầu cấm/gỡ ban tài khoản
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(BanAccData), // Truyền đối tượng BanAccData mà không cần bọc trong một đối tượng khác
  })
    .then((response) => {
      if (response.ok) {
        // Xử lý thành công, ví dụ: cập nhật giao diện người dùng
        banButton.classList.toggle("banned"); // Thay đổi lớp CSS của nút
        if (banButton.innerText === "Banned") {
          banButton.innerText = "inactive";
        } else {
          banButton.innerText = "Banned";
        }
      } else {
        // Xử lý lỗi

        alert("Lỗi khi cấm/gỡ ban tài khoản.");
      }
    })

    .catch((error) => {
      console.error("Lỗi khi gửi yêu cầu cấm/gỡ ban tài khoản:", error);
    });
}

const userList = document.getElementById("user-list");

// Gọi API để lấy danh sách người dùng
fetch("https://localhost:7281/api/Auth/DS")
  .then((response) => response.json())
  .then((data) => {
    // Xử lý dữ liệu người dùng trả về từ API
    data.forEach((user) => {
      const li = document.createElement("li");
      const banButton = document.createElement("button");
      banButton.innerText = `${user.status}`;
      banButton.onclick = () => {
        localStorage.removeItem("jwtToken_user");
        console.log(user.status);
        toggleBan(user.userName, banButton);
      };
      li.appendChild(document.createTextNode(user.userName + " "));
      li.appendChild(banButton);
      userList.appendChild(li);
    });
  })
  .catch((error) => {
    console.error("Lỗi khi gọi API:", error);
  });

// Xử lý chức năng cấm/gỡ ban tài khoản
