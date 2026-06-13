import { createContext, useContext, useState } from 'react';
const AuthContext = createContext(null);
function parseJwt(token) {
  const base64Url = token.split('.')[1];
  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  const jsonPayload = decodeURIComponent(
    atob(base64).split('').map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join('')
  );
  return JSON.parse(jsonPayload);
}
function buildUser(token) {
  try {
    const payload = parseJwt(token);
    return {
      id: parseInt(payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'], 10),
      email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
      role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
    };
  } catch {
    return null;
  }
}
export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const t = localStorage.getItem('token');
    return t ? buildUser(t) : null;
  });
  function login(token) {
    localStorage.setItem('token', token);
    setUser(buildUser(token));
  }
  function logout() {
    localStorage.removeItem('token');
    setUser(null);
  }
  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}
export function useAuth() {
  return useContext(AuthContext);
}
