using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace AgeOfChess
{
    class MultiplayerApiClient
    {
        public User AuthenticatedUser { get; set; }

        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public MultiplayerApiClient()
        {
            _baseUrl = "http://82.170.37.68:7275/";

            _client = new HttpClient()
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }

        public void RegisterUser(string username, string plainTextPassword)
        {
            string password = HashPassword(plainTextPassword);
            int userId = int.Parse(Get("register_user", new { username, password }));

            AuthenticatedUser = new User
            {
                Id = userId,
                Username = username,
                Password = password,
                LastElo = 1500
            };
        }

        public int? GetUserIdByName(string username)
        {
            string result = Get("get_user_id_by_name", new { username });
            return result != null ? int.Parse(result) : (int?)null;
        }

        public bool Login(string username, string plainTextPassword, string password = null)
        {
            if (password == null)
            {
                password = HashPassword(plainTextPassword);
            }

            string result = Get("login", new { username, password });

            if (result == null)
            {
                return false;
            }

            var dto = JsonConvert.DeserializeObject<UserDto>(result);

            AuthenticatedUser = new User
            {
                Id = dto.Id,
                Username = username,
                Password = password,
                LastElo = dto.LastElo
            };

            return true;
        }

        public void RefreshUser()
        {
            Login(AuthenticatedUser.Username, "", AuthenticatedUser.Password);
        }

        public int CreateLobby(MultiplayerGameSettings settings)
        {
            string result = Get("create_lobby", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,

                boardSize = settings.BoardSize,
                biddingEnabled = settings.BiddingEnabled,
                timeControlEnabled = settings.TimeControlEnabled,
                startTimeMinutes = settings.StartTimeMinutes,
                timeIncrementSeconds = settings.TimeIncrementSeconds,
                minRating = settings.MinRating,
                maxRating = settings.MaxRating,
                mapSeed = settings.MapSeed
            });

            return int.Parse(result);
        }

        public void CancelLobby(int id)
        {
            Get("cancel_lobby", new 
            { 
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                id 
            });
        }

        public int JoinLobby(Lobby lobby)
        {
            string result = Get("join_lobby", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                lobbyId = lobby.Id,
                mapSeed = lobby.Settings.MapSeed
            });

            return int.Parse(result);
        }

        public string GetGeneratedMapSeed(int lobbyId)
        {
            return Get("get_generated_seed", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                lobbyId
            });
        }

        public int GetGameId(int lobbyId)
        {
            string result = Get("get_game_id", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                lobbyId
            });

            return int.Parse(result);
        }

        public User GetOpponent(int gameId)
        {
            string result = Get("get_opponent", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                gameId
            });

            return JsonConvert.DeserializeObject<UserDto>(result).ToUser();
        }

        public void MakeBid(int gameId, int bid)
        {
            Get("make_bid", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                gameId,
                bid
            });
        }

        public void SetWhite(int gameId)
        {
            Get("set_white", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                gameId
            });
        }

        public void SetTime(bool isWhite, int gameId, int timeMs)
        {
            Get("set_time", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                gameId,
                isWhite,
                timeMs
            });
        }

        public PlayerTimesDto GetTimes(int gameId)
        {
            string result = Get("get_player_times", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                gameId
            });

            return JsonConvert.DeserializeObject<PlayerTimesDto>(result);
        }

        public List<Lobby> GetActiveLobbies()
        {
            string result = Get("get_open_lobbies", new 
            { 
                username = AuthenticatedUser.Username, 
                password = AuthenticatedUser.Password 
            });

            if (result == null)
            {
                return new List<Lobby>();
            }

            List<LobbyDto> dtos = JsonConvert.DeserializeObject<List<LobbyDto>>(result);

            return dtos.Select(e => e.ToLobby()).ToList();
        }

        public PollLatestMoveDto LatestMove(int gameId)
        {
            string result = Get("poll_last_move", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                gameId
            });

            if (result == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<PollLatestMoveDto>(result);
        }

        public void MakeMove(MakeMoveDto dto)
        {
            Get("make_move", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,

                gameId = dto.GameId,
                moveNumber = dto.MoveNumber,
                sourceSquare = dto.SourceSquare,
                destinationSquare = dto.DestinationSquare,
                piecePlaced = dto.PiecePlaced,
                objectCaptured = dto.ObjectCaptured,
                timeMs = dto.TimeMs,
                isWhite = dto.IsWhite
            });
        }

        public void SetResult(int gameId, string result)
        {
            Get("set_result", new
            {
                username = AuthenticatedUser.Username,
                password = AuthenticatedUser.Password,
                gameId,
                result
            });
        }

        private string Get(string url, object convertableObject = null)
        {
            HttpRequestMessage request;
            if (convertableObject != null)
            {
                var jsonString = JsonConvert.SerializeObject(convertableObject);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                request = new HttpRequestMessage(HttpMethod.Get, url) { Content = content };
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Get, url);
            }

            var response = _client.SendAsync(request).Result;
            var stringResult = response.Content.ReadAsStringAsync().Result;

            return stringResult == "None" ? null : stringResult;
        }

        private string HashPassword(string password)
        {
            // TODO
            return password;
        }
    }
}
