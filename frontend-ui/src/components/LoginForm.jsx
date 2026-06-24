import React, { useState } from 'react';

const LoginForm = ({ onToggle }) => {
  const [formData, setFormData] = useState({ email: '', password: '' });

  const handleSubmit = (e) => { e.preventDefault(); /* Gọi API login ở đây */ };

  return (
    <div className="bg-[#FDFBF7] p-8 w-full max-w-md">
      <h2 className="text-2xl font-serif text-[#4A3F35] mb-0">Bliss Events</h2>
      <h1 className="text-3xl font-serif text-black italic mb-2">Chào mừng trở lại</h1>
      <p className="text-sm text-gray-700 mb-6">Đăng nhập để đặt lịch và quản lý sự kiện.</p>

      <div className="flex mb-6 border border-[#D1CBC1]">
        <button className="flex-1 py-3 bg-[#7A2836] text-white font-medium">Đăng nhập</button>
        <button onClick={onToggle} className="flex-1 py-3 bg-[#EBE5D9] text-[#4A3F35] font-medium border-l border-[#D1CBC1]">Đăng ký</button>
      </div>

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block text-lg font-serif text-black mb-1">Email</label>
          <input className="w-full p-2 bg-[#EBE5D9] border-none outline-none" onChange={(e) => setFormData({...formData, email: e.target.value})} />
        </div>
        <div>
          <label className="block text-lg font-serif text-black mb-1">Mật khẩu</label>
          <input type="password" className="w-full p-2 bg-[#EBE5D9] border-none outline-none" onChange={(e) => setFormData({...formData, password: e.target.value})} />
        </div>
        <button className="w-full py-4 mt-6 bg-[#7A2836] text-white font-bold tracking-wider hover:bg-[#63202C]">Đăng nhập</button>
      </form>

      <p className="text-center text-sm mt-6 text-[#4A3F35]">
        Chưa có tài khoản? <span onClick={onToggle} className="text-[#A6886A] font-bold cursor-pointer hover:underline">Đăng ký</span>
      </p>
    </div>
  );
};

export default LoginForm;