﻿namespace TaskMate.DTOs.Auth;

public record TokenResponseDTO(string token,
                               DateTime expireDate,
                               DateTime refreshTokenExpration,
                               string refreshToken,
                               string username,
                               string email,
                               string appuserid);