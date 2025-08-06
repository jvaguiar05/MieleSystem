# 🛡️ Identity Context – MieleSystem.Domain.Identity

O contexto `Identity` é responsável por **gerenciar usuários**, autenticação, autorização, tokens de sessão, códigos OTP e eventos relacionados à administração de contas. Ele representa o coração da segurança e controle de acesso no MieleSystem.

---

## 📂 Estrutura de Diretórios

```plaintext
MieleSystem.Domain/
└── Identity/
    ├── Entities/
    │   ├── User.cs                    # Aggregate Root (centraliza tokens, OTP e status)
    │   ├── RefreshToken.cs           # Entidade subordinada a User
    │   └── OtpSession.cs             # Entidade vinculada a User (mas com ciclo próprio)
    │
    ├── ValueObjects/
    │   ├── Email.cs                  # VO validado e normalizado
    │   ├── PasswordHash.cs           # VO encapsulando hash seguro
    │   ├── Token.cs                  # VO representando o valor seguro do token
    │   └── OtpCode.cs                # VO encapsulando código OTP e seu comportamento
    │
    ├── Events/
    │   ├── User/                     # Eventos relacionados a modificações do usuário
    │   │   ├── UserRegisteredEvent.cs
    │   │   ├── UserUpdatedEvent.cs
    │   │   ├── UserDeletedEvent.cs
    │   │   ├── PasswordChangedEvent.cs
    │   │   └── UserRoleChangedEvent.cs
    │   │
    │   ├── Auth/                     # Eventos relacionados à autenticação
    │   │   ├── UserLoggedInEvent.cs
    │   │   ├── UserLoggedOutEvent.cs
    │   │   └── OtpCodeRequestedEvent.cs
    │   │
    │   └── Admin/                    # Eventos relacionados à administração do sistema
    │       ├── ViewerUserInvitedEvent.cs
    │       ├── UserRegistrationApprovedEvent.cs
    │       └── UserRegistrationRejectedEvent.cs
    │
    ├── Repositories/
    │   ├── IUserRepository.cs        # Interface para acesso e consulta ao Aggregate User
    │   ├── IRefreshTokenRepository.cs # Operações específicas para sessões e revogação
    │   └── IOtpSessionRepository.cs  # Consulta e manutenção de sessões OTP
    │
    ├── Services/
    │   ├── IPasswordHasher.cs        # Hash e verificação de senhas
    │   ├── ITokenService.cs          # Geração e validação de tokens JWT/Refresh
    │   └── IOtpService.cs            # Geração, envio e validação de OTPs
    │
    └── Enums/
        ├── UserRole.cs               # Smart Enum com níveis de permissão (Admin, Viewer...)
        └── UserRegistrationSituation.cs # Situação de aprovação da conta (Pending, Accepted, Rejected)

```

---

## 🧩 Entities

| Entidade       | Descrição                                                                                                                           |
| -------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| `User`         | Aggregate Root. Contém nome, e-mail, senha, cargo (`UserRole`), tokens e sessões OTP. Manipula regras de negócio e dispara eventos. |
| `RefreshToken` | Entidade subordinada. Representa sessões persistentes de login (via JWT Refresh Token). Pode ser revogada e expirada.               |
| `OtpSession`   | Entidade independente vinculada ao `User`. Armazena e valida sessões de código OTP. Pode ser auditada e expirada.                   |

---

## 🎯 Value Objects

| VO             | Responsabilidade                                                     |
| -------------- | -------------------------------------------------------------------- |
| `Email`        | Encapsula um e-mail válido e normalizado.                            |
| `PasswordHash` | Armazena o hash seguro da senha, com restrições.                     |
| `OtpCode`      | Representa um código OTP e sua validade.                             |
| `Token`        | Representa o valor seguro de um refresh token (string única, opaca). |

---

## 📡 Events

Os eventos são agrupados por significado semântico:

### 🔸 User Events

| Evento                 | Descrição                                |
| ---------------------- | ---------------------------------------- |
| `UserRegisteredEvent`  | Disparado ao registrar um novo usuário.  |
| `UserUpdatedEvent`     | Disparado ao atualizar dados do usuário. |
| `UserDeletedEvent`     | Disparado no soft delete do usuário.     |
| `PasswordChangedEvent` | Disparado após troca de senha.           |
| `UserRoleChangedEvent` | Disparado ao promover ou alterar cargo.  |

### 🔹 Auth Events

| Evento                  | Descrição                            |
| ----------------------- | ------------------------------------ |
| `UserLoggedInEvent`     | Disparado após login bem-sucedido.   |
| `UserLoggedOutEvent`    | Disparado no logout do usuário.      |
| `OtpCodeRequestedEvent` | Disparado ao solicitar envio de OTP. |

### 🔸 Admin Events

| Evento                          | Descrição                                        |
| ------------------------------- | ------------------------------------------------ |
| `ViewerUserInvitedEvent`        | Envia convite para criar conta Viewer (leitura). |
| `UserRegistrationApprovedEvent` | Conta aprovada pelo Admin.                       |
| `UserRegistrationRejectedEvent` | Conta rejeitada pelo Admin.                      |

---

## 📦 Repositories

| Repositório               | Descrição                                                                                           |
| ------------------------- | --------------------------------------------------------------------------------------------------- |
| `IUserRepository`         | Consulta e persistência do `User`. Inclui buscas por e-mail, registros pendentes, contas expiradas. |
| `IRefreshTokenRepository` | Consulta por token, sessão e limpeza de tokens inválidos.                                           |
| `IOtpSessionRepository`   | Consulta por código, sessão ou status OTP.                                                          |

---

## 🛠 Services

| Serviço           | Responsabilidade                                  |
| ----------------- | ------------------------------------------------- |
| `IPasswordHasher` | Criptografar e verificar senhas com segurança.    |
| `ITokenService`   | Criar e validar JWT + Refresh Tokens.             |
| `IOtpService`     | Gerar, enviar e validar códigos OTP (via e-mail). |

---

## 🧾 Enums

| Enum                        | Uso                                                  |
| --------------------------- | ---------------------------------------------------- |
| `UserRole`                  | Define o cargo: Admin, Editor, Viewer, Suspended.    |
| `UserRegistrationSituation` | Define status da conta: Pending, Accepted, Rejected. |

---

## 📌 Convenções

- O `User` dispara eventos relevantes como parte do ciclo de vida (registro, update, promoção).
- Eventos relacionados à autenticação e administração são disparados nos Handlers da camada Application.
- Todos os dados sensíveis (senhas, tokens, códigos) são encapsulados em Value Objects.
- Soft delete é tratado por `ISoftDeletable` e refletido em `IsDeleted` / `DeletedAt`.

---

## ✅ Fluxos Suportados

- Criação e aprovação de novos usuários
- Login com Refresh Token
- Envio e validação de OTP
- Suspensão automática de contas expiradas
- Convite de usuários Viewer via Admin
- Auditoria e rastreabilidade por eventos