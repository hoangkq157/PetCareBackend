# PetCareBackend - Web API

Nhóm 12 | Môn Công nghệ .NET | Quản lý Chăm sóc Thú cưng

---

## Cấu trúc project

```
PetCareBackend/
├── Models/
│   ├── NhanVien.cs
│   ├── ChuNuoi.cs
│   ├── ThuCung.cs
│   ├── DichVu.cs
│   ├── LichHen.cs
│   ├── LichHenDichVu.cs
│   ├── TiemPhong.cs
│   └── HoaDon.cs
├── Data/
│   └── AppDbContext.cs
├── Controllers/
│   ├── NhanVienController.cs
│   ├── ChuNuoiController.cs
│   ├── ThuCungController.cs
│   ├── DichVuController.cs
│   ├── LichHenController.cs
│   ├── LichHenDichVuController.cs
│   ├── TiemPhongController.cs
│   └── HoaDonController.cs
├── Properties/
│   └── launchSettings.json
├── Program.cs
├── appsettings.json
└── PetCareBackend.csproj
```

---

## Bước 1 – Mở project

```bash
cd PetCareBackend
code .
```

---

## Bước 2 – Đổi connection string

Mở file `appsettings.json`, sửa dòng:

```
"Server=TÊN_SERVER_CỦA_BẠN;Database=PetCareDB;..."
```

Thay `TÊN_SERVER_CỦA_BẠN` bằng tên SQL Server instance trên máy bạn.
Ví dụ: `DESKTOP-AJA6TML`, `localhost`, `.\SQLEXPRESS`

---

## Bước 3 – Cài packages & chạy

```bash
dotnet restore
dotnet run
```

API chạy tại: `http://localhost:5000`

---

## Danh sách API endpoints

### NhanVien
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | /api/nhanvien | Lấy tất cả nhân viên |
| GET | /api/nhanvien/{id} | Lấy theo ID |
| POST | /api/nhanvien | Thêm nhân viên mới |
| PUT | /api/nhanvien/{id} | Cập nhật |
| DELETE | /api/nhanvien/{id} | Xóa |
| POST | /api/nhanvien/login | Đăng nhập |

### ChuNuoi
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | /api/chuNuoi | Lấy tất cả chủ nuôi |
| GET | /api/chuNuoi/{id} | Lấy theo ID |
| POST | /api/chuNuoi | Thêm mới |
| PUT | /api/chuNuoi/{id} | Cập nhật |
| DELETE | /api/chuNuoi/{id} | Xóa |

### ThuCung
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | /api/thuCung | Lấy tất cả thú cưng |
| GET | /api/thuCung/{id} | Lấy theo ID |
| GET | /api/thuCung/bychuNuoi/{maCN} | Lấy thú cưng theo chủ nuôi |
| POST | /api/thuCung | Thêm mới |
| PUT | /api/thuCung/{id} | Cập nhật |
| DELETE | /api/thuCung/{id} | Xóa |

### DichVu
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | /api/dichVu | Lấy dịch vụ đang hoạt động |
| GET | /api/dichVu/{id} | Lấy theo ID |
| GET | /api/dichVu/danhmuc/{danhMuc} | Lọc theo danh mục (Spa, CatTia, LuuTru...) |
| POST | /api/dichVu | Thêm mới |
| PUT | /api/dichVu/{id} | Cập nhật |
| DELETE | /api/dichVu/{id} | Xóa |

### LichHen
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | /api/lichHen | Lấy tất cả lịch hẹn |
| GET | /api/lichHen/{id} | Lấy theo ID |
| GET | /api/lichHen/bythuCung/{maTC} | Lịch hẹn theo thú cưng |
| GET | /api/lichHen/bytrangThai/{trangThai} | Lọc theo trạng thái |
| POST | /api/lichHen | Đặt lịch hẹn mới |
| PUT | /api/lichHen/{id} | Cập nhật |
| PATCH | /api/lichHen/{id}/trangthai | Chỉ đổi trạng thái |
| DELETE | /api/lichHen/{id} | Xóa |

### TiemPhong
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | /api/tiemPhong | Lấy tất cả |
| GET | /api/tiemPhong/{id} | Lấy theo ID |
| GET | /api/tiemPhong/bythuCung/{maTC} | Lịch sử tiêm theo thú cưng |
| GET | /api/tiemPhong/sapdenhan | Cảnh báo sắp đến hạn tiêm (30 ngày) |
| POST | /api/tiemPhong | Thêm mới (tự tính ngày tiêm tiếp) |
| PUT | /api/tiemPhong/{id} | Cập nhật |
| DELETE | /api/tiemPhong/{id} | Xóa |

### HoaDon
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | /api/hoaDon | Lấy tất cả hoá đơn |
| GET | /api/hoaDon/{id} | Lấy theo ID |
| GET | /api/hoaDon/bychuNuoi/{maCN} | Hoá đơn theo chủ nuôi |
| GET | /api/hoaDon/chuathanhtoan | Hoá đơn chưa thanh toán |
| POST | /api/hoaDon | Tạo hoá đơn mới |
| PUT | /api/hoaDon/{id} | Cập nhật |
| PATCH | /api/hoaDon/{id}/thanhtoan | Đánh dấu đã thanh toán |
| DELETE | /api/hoaDon/{id} | Xóa |

---

## Gọi API từ Angular

```typescript
// Ví dụ service trong Angular
getThuCung() {
  return this.http.get('http://localhost:5000/api/thuCung');
}

getLichHenChoDuyet() {
  return this.http.get('http://localhost:5000/api/lichHen/bytrangThai/ChoDuyet');
}
```
