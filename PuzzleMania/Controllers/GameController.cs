using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PuzzleMania.Helpers;
using PuzzleMania.Interfaces;
using PuzzleMania.Models;
using PuzzleMania.Repositories;
using PuzzleMania.ViewModels;

namespace PuzzleMania.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRiddleRepository _riddleRepository;

        public GameController(IGameRepository gameRepository, ITeamRepository teamRepository, IHttpContextAccessor httpContextAccessor, IRiddleRepository riddleRepository)
        {
            _gameRepository = gameRepository;
            _teamRepository = teamRepository;
            _httpContextAccessor = httpContextAccessor;
            _riddleRepository = riddleRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StartGame()
        {
            //check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = $"You must be logged in to access this page.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            //retriving current user
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            //check if user has a team
            if (!await _teamRepository.CheckIfUserHasTeam(currentUserId))
            {
                TempData["Message"] = "You have not joined a team yet.";
                return RedirectToAction("JoinTeam", "Team");
            }

            bool isTeamIncomplete =  _gameRepository.IsTeamIncomplete(currentUserId);

            //check if current team is incomplete
            if (isTeamIncomplete)
            {
                TempData["Message"] = "Your team is incomplete, wait for users to join";
                return RedirectToAction("TeamStats", "Team");
            }

            //retriving the team of current user
            var team = await _teamRepository.GetTeamByUserId(currentUserId);


            var startGameViewModel = new StartGameViewModel
            {
                TeamName = team.TeamName
            };

            return View(startGameViewModel);
        }

        //TEST VVV TODO - Tu skończyłem
        [HttpPost]
        public async Task<IActionResult> StartGamePost()
        {
            // Get the current team ID
            //var curTeamId = await _teamRepository.GetByIdAsync(teamId);

            //1.Retrive current user
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            //2.Retrive the teamId of current user
            var curTeamId = await _teamRepository.GetTeamByUserId(currentUserId);
            //3. Assaign teamid to variable
            int teamId = curTeamId.TeamId;
            // Add a new game and get the generated gameId
            int gameId = _gameRepository.AddGame(teamId).GameId;

            /*List<int> availableRiddleIds = await _riddleRepository.GetAvailableRiddleIds();

            if (availableRiddleIds.Count == 0)
                // Handle the case when no riddles are available
                return NotFound();*/


            // Select a random riddle ID
            // Random random = new Random();
            //int randomRiddleId = availableRiddleIds[random.Next(availableRiddleIds.Count)];
            int randomRiddleId = await GetNextRandomRiddleId();

            // Assign the random riddle ID to the current game
            await _riddleRepository.AssignRiddleId(gameId, randomRiddleId);
            // Redirect the user to the first riddle of the game
            return RedirectToAction("Riddle", new { gameId = gameId, riddleId = randomRiddleId });
        }

        [HttpGet("/game/{gameId}/riddle/{riddleId}")]
        public async Task<IActionResult> Riddle(int gameId, int riddleId, string? answer = null)
        {
            //TEST
            // Check if the method was accessed from SubmitAnswer
           // bool accessedFromSubmitAnswer = TempData.ContainsKey("AccessedFromSubmitAnswer") && (bool)TempData["AccessedFromSubmitAnswer"];

            // Clear the TempData value
            TempData.Remove("AccessedFromSubmitAnswer");


            // Get the riddle information based on the gameId and riddleId
            var riddle = await _riddleRepository.GetByIdAsync(gameId, riddleId);

            // Handle the case when the riddle doesn't exist
            if (riddle == null)
                return NotFound();

        /*    if(accessedFromSubmitAnswer)
            {
                // Set a ViewBag property to indicate it was accessed from SubmitAnswer
                ViewBag.AccessedFromSubmitAnswer = true;

                return RedirectToAction("Riddle", new { gameId = gameId, riddleId = riddleId });
            }*/

         // Pass the riddle data to the view
            return View("Riddle", riddle);
        }

        [HttpPost("/game/{gameId}/riddle/{riddleId}")]
        public async Task<IActionResult> SubmitAnswer(int gameId, int riddleId, string answer)
        {
            // Get the riddle information based on the gameId and riddleId
            var riddle = await _riddleRepository.GetByIdAsync(gameId, riddleId);

            // Handle the case when the riddle doesn't exist
            if (riddle == null)
                return NotFound();

            // Check if the submitted answer is correct
            bool isCorrect = CheckAnswer(answer, riddle.Answer);
            int nextRandomRiddleId = await GetNextRandomRiddleId(); //gameid

            // Retrieve the completedRiddlesCount from session state
            int? completedRiddlesCount = _httpContextAccessor.HttpContext.Session.GetInt32("CompletedRiddlesCount") ?? 0;

            if (isCorrect)
            {
                // Display a success message
                ViewBag.Message = "Correct answer!";

                // Increment the count of completed riddles
                completedRiddlesCount++;

                // Check if all riddles have been completed
                if (completedRiddlesCount >= 3) // Adjust the condition based on the desired number of riddles
                {
                    // Redirect to the finish game view
                    return View("FinishGame");
                }

                // Proceed to the next random riddle

                // Set the ViewBag properties
                ViewBag.ShowNextButton = true;
                ViewBag.NextRiddleId = nextRandomRiddleId;


                // Store the updated completedRiddlesCount in session state
                _httpContextAccessor.HttpContext.Session.SetInt32("CompletedRiddlesCount", completedRiddlesCount.Value);

                // Redirect to the next riddle
                //TODO - It should return view here as we don't get the incorrect info answer in the view
                await _riddleRepository.AssignRiddleId(gameId, nextRandomRiddleId);

                //TEST
               // TempData["AccessedFromSubmitAnswer"] = true;

                return await Riddle(gameId, nextRandomRiddleId);
            }
            else
            {
                // Display the correct answer
                ViewBag.Message = "Incorrect answer. The correct answer is: " + riddle.Answer;

                // Set the ViewBag properties
                ViewBag.ShowNextButton = true;
                ViewBag.NextRiddleId = nextRandomRiddleId;

                // Increment the count of completed riddles
                completedRiddlesCount++;

                if (completedRiddlesCount >= 3) // Adjust the condition based on the desired number of riddles
                {
                    // Redirect to the finish game view
                    return View("FinishGame");
                }

                //ViewBag.NextRiddleId = riddleId; // Stay on the same riddle
            }

    

            // Pass the riddle data and the submitted answer to the view
            ViewBag.SubmittedAnswer = answer;

            // Store the current completedRiddlesCount in session state
            _httpContextAccessor.HttpContext.Session.SetInt32("CompletedRiddlesCount", completedRiddlesCount.Value);

            // return View("Riddle", riddleModel);
            //TODO - It should return view here as we don't get the incorrect info answer in the view
            await _riddleRepository.AssignRiddleId(gameId, nextRandomRiddleId);

            //TEST
            TempData["AccessedFromSubmitAnswer"] = true;

            return await Riddle(gameId, nextRandomRiddleId);
        }

        [HttpGet]
        public async Task<IActionResult> FinishGame()
        {
            /*        // Get the current team ID
                    var teamId = await _teamRepository.GetTeamIdByUserId(_httpContextAccessor.HttpContext.User.GetUserId());

                    // Get the team name
                    var teamName = await _teamRepository.GetTeamNameById(teamId);

                    // Get the team score
                    var teamScore = await _gameRepository.GetTeamScore(teamId);

                    // Pass the team name and score to the view
                    var finishGameViewModel = new FinishGameViewModel
                    {
                        TeamName = teamName,
                        TeamScore = teamScore
                    };
        */
            //return View(finishGameViewModel);
            return View();
        }

        //httpget i post w jednym rozdzielic todo
        /* [HttpGet("/game/{gameId}/riddle/{riddleId}")]
         public async Task<IActionResult> Riddle(int gameId, int riddleId, string usersAnswer)
         {
             // Get the riddle information based on the gameId and riddleId
             var riddle = await _riddleRepository.GetByIdAsync(gameId, riddleId);

             // Handle the case when the riddle doesn't exist
             if (riddle == null)
                 return NotFound();

             // Check if the user has submitted an answer
             string submittedAnswer = usersAnswer;

             if (!string.IsNullOrEmpty(submittedAnswer))
             {
                 // Check if the submitted answer is correct
                 bool isCorrect = CheckAnswer(submittedAnswer, riddle.Answer);

                 if (isCorrect)
                 {
                     // Display a success message
                     ViewBag.Message = "Correct answer!";

                     // Increment the count of completed riddles
                     int completedRiddlesCount = Convert.ToInt32(TempData["CompletedRiddlesCount"]) + 1;

                     // Check if all riddles have been completed
                     if (completedRiddlesCount >= 3) // Adjust the condition based on the desired number of riddles
                     {
                         // Redirect to the finish game view
                         return RedirectToAction("FinishGame");
                     }

                     // Proceed to the next random riddle
                     int nextRandomRiddleId = await GetNextRandomRiddleId(); //gameid

                     // Redirect to the next riddle
                     return RedirectToAction("Riddle", new { gameId = gameId, riddleId = nextRandomRiddleId });
                 }
                 else
                 {
                     // Display the correct answer
                     ViewBag.Message = "Incorrect answer. The correct answer is: " + riddle.Answer;
                 }
             }
             else
             {
                 // Reset the count of completed riddles when starting a new riddle
                 TempData["CompletedRiddlesCount"] = 0;
             }

             // Pass the riddle data and the submitted answer to the view
             ViewBag.SubmittedAnswer = submittedAnswer;
             return View(riddle);
         }*/




        private bool CheckAnswer(string submittedAnswer, string correctAnswer)
        {
            // Implement your answer validation logic here
            // Compare the submitted answer with the correct answer and return true if they match
            // You can use case-insensitive comparison or apply any specific validation rules you need

            bool isCorrect = string.Equals(submittedAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase);
            return isCorrect;

        }

        private async Task<int> GetNextRandomRiddleId() //gameid
        {
            List<int> availableRiddleIds = await _riddleRepository.GetAvailableRiddleIds(); //gameid


            if (availableRiddleIds.Count == 0)
            {
                // Handle the case when no riddles are available
                throw new Exception("No riddles are available.");
            }

            // Select a random riddle ID
            Random random = new Random();
            int randomRiddleId = availableRiddleIds[random.Next(availableRiddleIds.Count)];

            // Assign the random riddle ID to the current game

            return randomRiddleId;
        }


        /* [HttpGet("/game/{gameId}/riddle/{riddleId}")]
         public async Task<IActionResult> Riddle(int gameId, int riddleId)
         {
             // Get the riddle information based on the gameId and riddleId
             var riddle = await _riddleRepository.GetByIdAsync(gameId, riddleId);

             // Handle the case when the riddle doesn't exist
             if (riddle == null)
                 return NotFound();

             // Check if the user has submitted an answer
             string submittedAnswer = Request.Form["answer"];

             if (!string.IsNullOrEmpty(submittedAnswer))
             {
                 // Check if the submitted answer is correct
                 bool isCorrect = CheckAnswer(submittedAnswer, riddle.Answer);

                 if (isCorrect)
                 {
                     // Display a success message
                     ViewBag.Message = "Correct answer!";

                     // Proceed to the next riddle
                     List<int> availableRiddleIds = await _riddleRepository.GetAvailableRiddleIds();

                     if (availableRiddleIds.Count == 0)
                         // Handle the case when no riddles are available
                         return NotFound();

                     Random random = new Random();
                     int nextRandomRiddleId = availableRiddleIds[random.Next(availableRiddleIds.Count)];


                     // Check if there are more riddles in the game
                     if (nextRiddleId <= 3) // Adjust the condition based on the total number of riddles
                     {
                         // Redirect to the next riddle
                         return RedirectToAction("Riddle", new { gameId = gameId, riddleId = nextRandomRiddleId });
                     }
                     else
                     {
                         // Handle the case when all riddles have been completed
                         ViewBag.Message = "Congratulations! You have completed all the riddles.";
                         // You can redirect to a different action or display a different view here
                         return View("FinishGame");
                     }
                 }
                 else
                 {
                     // Display the correct answer
                     ViewBag.Message = "Incorrect answer. The correct answer is: " + riddle.Answer;
                 }
             }

             // Pass the riddle data to the view
             return View(riddle);
         }*/


        /* [HttpGet("/game/{gameId}/riddle/{riddleId}")]
         public async Task<IActionResult> Riddle(int gameId, int riddleId)
         {
             // Check if it's a new game (no riddles assigned to the game yet)
             bool isNewGame = await _riddleRepository.IsNewGame(gameId);

             if (isNewGame)
             {
                 // Assign the riddle ID to the current game
                 await _riddleRepository.AssignRiddleId(gameId, riddleId);

                 // Get the riddle information based on the gameId and riddleId
                 var riddle = await _riddleRepository.GetByIdAsync(gameId, riddleId);

                 // Handle the case when the riddle doesn't exist
                 if (riddle == null)
                     return NotFound();

                 // Redirect to the view
                 return View(riddle);


             }
             return RedirectToAction("StartGame");



         }*/




        /*     [HttpGet("/game/{gameId}/riddle/{riddleId}")]
             public async Task<IActionResult> Riddle(int gameId, int riddleId)
             {
                 // Get the riddle information based on the gameId and riddleId
                 var riddle = await _riddleRepository.GetByIdAsync(gameId, riddleId);

                 if (riddle == null)           
                     return NotFound();


                 // Pass the riddle data to the view
                 return View(riddle);
             }*/










    }






}
