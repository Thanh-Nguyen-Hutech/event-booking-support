import React, { useState } from 'react';

const RegisterForm = () => {
  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    password: '',
    roleId: 1 // Mặc định là 1 (Khách hàng) theo DTO
  });

  const [errors, setErrors] = useState({});
  const [serverMessage, setServerMessage] = useState('');

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ 
      ...formData, 
      [name]: name === 'roleId' ? parseInt(value, 10) : value 
    });
    if (errors[name]) setErrors({ ...errors, [name]: '' });
  };

  const validateForm = () => {
    let newErrors = {};
    
    if (!formData.fullName.trim()) {
      newErrors.fullName = 'Họ và tên không được để trống.';
    } else if (formData.fullName.length > 255) {
      newErrors.fullName = 'Họ và tên không được vượt quá 255 ký tự.';
    }

    if (!formData.email.trim()) {
      newErrors.email = 'Email không được để trống.';
    } else if (!formData.email.match(/^[\w-.]+@([\w-]+\.)+[\w-]{2,4}$/)) {
      newErrors.email = 'Email không đúng định dạng.';
    }

    if (!formData.password) {
      newErrors.password = 'Mật khẩu không được để trống.';
    } else if (formData.password.length < 6) {
      newErrors.password = 'Mật khẩu phải có ít nhất 6 ký tự.';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;

    try {
      const response = await fetch('https://localhost:7201/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          FullName: formData.fullName,
          Email: formData.email,
          Password: formData.password,
          RoleId: formData.roleId
        })
      });

      const data = await response.json();
      if (response.ok) {
        setServerMessage(data.message);
        // Reset form hoặc chuyển hướng người dùng sang trang đăng nhập
      } else {
        setServerMessage(data.Message || 'Đăng ký thất bại.');
      }
    } catch (error) {
      setServerMessage('Không thể kết nối đến máy chủ.');
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-[#FDFBF7] p-4">
      <div className="w-full max-w-md bg-[#FDFBF7] p-8 rounded-lg shadow-sm border border-[#E8E1D5]">
        
        <div className="mb-6">
          <h2 className="text-[#C49A45] text-xl font-serif mb-1">Bliss Events</h2>
          <h1 className="text-3xl font-serif text-black mb-2">Tạo tài khoản</h1>
        </div>

        {serverMessage && (
          <div className="mb-4 p-3 bg-gray-100 text-sm font-medium text-center rounded">
            {serverMessage}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4 text-[#1A1A1A]">
          
          <div>
            <label className="block text-lg font-serif mb-1">Họ và tên *</label>
            <input 
              type="text" name="fullName" value={formData.fullName} onChange={handleChange}
              className="w-full p-2 bg-[#EBE5D9] border border-[#D1C9B9] focus:outline-none focus:border-[#7A2836]" 
            />
            {errors.fullName && <p className="text-red-500 text-xs mt-1">{errors.fullName}</p>}
          </div>

          <div>
            <label className="block text-lg font-serif mb-1">Email *</label>
            <input 
              type="text" name="email" value={formData.email} onChange={handleChange}
              className="w-full p-2 bg-[#EBE5D9] border border-[#D1C9B9] focus:outline-none focus:border-[#7A2836]" 
            />
            {errors.email && <p className="text-red-500 text-xs mt-1">{errors.email}</p>}
          </div>

          <div>
            <label className="block text-lg font-serif mb-1">Mật khẩu *</label>
            <input 
              type="password" name="password" value={formData.password} onChange={handleChange}
              className="w-full p-2 bg-[#EBE5D9] border border-[#D1C9B9] focus:outline-none focus:border-[#7A2836]" 
            />
            {errors.password && <p className="text-red-500 text-xs mt-1">{errors.password}</p>}
          </div>

          <div>
            <label className="block text-lg font-serif mb-1">Vai Trò *</label>
            <select 
              name="roleId" value={formData.roleId} onChange={handleChange}
              className="w-full p-2 bg-[#EBE5D9] border border-[#D1C9B9] focus:outline-none focus:border-[#7A2836] h-[42px]"
            >
              <option value={1}>Khách hàng đặt lịch</option>
              <option value={2}>Thợ chụp ảnh / Đối tác</option>
            </select>
          </div>

          <button 
            type="submit" 
            className="w-full mt-4 py-3 bg-[#7A2836] text-white font-serif text-xl hover:bg-[#5E1E29] transition-colors"
          >
            Tạo tài khoản
          </button>

        </form>
      </div>
    </div>
  );
};

export default RegisterForm;