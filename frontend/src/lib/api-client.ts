const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5091/api';

/**
 * Helper function to create headers with optional Authorization token
 */
export const createHeaders = (token?: string | null): HeadersInit => {
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
  };

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  return headers;
};

/**
 * Helper function to make authenticated API requests
 */
export const apiClient = {
  async get<T>(url: string, token?: string | null): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${url}`, {
      method: 'GET',
      headers: createHeaders(token),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => null);
      const message = errorData?.message || response.statusText || 'Request failed';
      throw new Error(message);
    }

    return response.json();
  },

  async post<T>(url: string, data: unknown, token?: string | null): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${url}`, {
      method: 'POST',
      headers: createHeaders(token),
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Request failed' }));
      throw new Error(errorData.message || 'Request failed');
    }

    return response.json();
  },

  async put<T>(url: string, data: unknown, token?: string | null): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${url}`, {
      method: 'PUT',
      headers: createHeaders(token),
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Request failed' }));
      throw new Error(errorData.message || 'Request failed');
    }

    return response.json();
  },

  async delete(url: string, token?: string | null): Promise<void> {
    const response = await fetch(`${API_BASE_URL}${url}`, {
      method: 'DELETE',
      headers: createHeaders(token),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => null);
      const message = errorData?.message || response.statusText || 'Request failed';
      throw new Error(message);
    }
  },
};

export { API_BASE_URL };
