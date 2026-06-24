import React, { useState } from 'react';

const RegisterForm = ({ onToggle }) => {
  const [formData, setFormData] = useState({ fullName: '', email: '', password: '', roleId: 1 });

  const handleSubmit = async (e) => { e.preventDefault(); /* Gọi API register ở đây */ };

  return (
    <div className="w-full max-w-md bg-[#FDFBF7] p-8">
      <h2 className="text-[#C49A45] text-xl font-serif mb-1">Bliss Events</h2>
      <h1 className="text-3xl font-serif text-black mb-6">Tạo tài khoản</h1>

      <div className="flex mb-6 border border-[#D1CBC1]">
        <button onClick={onToggle} className="flex-1 py-3 bg-[#EBE5D9] text-[#4A3F35] font-medium border-r border-[#D1CBC1]">Đăng nhập</button>
        <button className="flex-1 py-3 bg-[#7A2836] text-white font-medium">Đăng ký</button>
      </div>

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block text-lg font-serif mb-1">Họ và tên *</label>
          <input type="text" className="w-full p-2 bg-[#EBE5D9] border outline-none" onChange={(e) => setFormData({...formData, fullName: e.target.value})} />
        </div>
        <div>
          <label className="block text-lg font-serif mb-1">Email *</label>
          <input type="text" className="w-full p-2 bg-[#EBE5D9] border outline-none" onChange={(e) => setFormData({...formData, email: e.target.value})} />
        </div>
        <div>
          <label className="block text-lg font-serif mb-1">Mật khẩu *</label>
          <input type="password" className="w-full p-2 bg-[#EBE5D9] border outline-none" onChange={(e) => setFormData({...formData, password: e.target.value})} />
        </div>
        <button type="submit" className="w-full mt-4 py-3 bg-[#7A2836] text-white font-serif text-xl">Tạo tài khoản</button>
      </form>

      <p className="text-center text-sm mt-6 text-[#4A3F35]">
        Đã có tài khoản? <span onClick={onToggle} className="text-[#A6886A] font-bold cursor-pointer hover:underline">Đăng nhập</span>
      </p>
    </div>
  );
};

export default RegisterForm;