// Assets/Scripts/Auth/AuthResponse.cs
namespace YourGame.Auth
{
    [System.Serializable]
    public class AuthResponse
    {
        public string status;   // "ok" or "error"
        public string message;  // サーバーからのメッセージ
        public string token;    // 将来JWT用（未使用ならnullでOK）
        public int userId;      // ユーザーID
        public string email;    // メールアドレス
    }
}
