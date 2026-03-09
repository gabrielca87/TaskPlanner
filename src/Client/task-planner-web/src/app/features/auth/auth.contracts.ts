export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterRequest = {
  email: string;
  displayName: string;
  password: string;
};

export type AuthResponse = {
  accessToken: string;
  userId: string;
  email: string;
  displayName: string;
};
