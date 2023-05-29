﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuzzleMania.Data;
using PuzzleMania.Helpers;
using PuzzleMania.Interfaces;
using PuzzleMania.Models;
using PuzzleMania.Repositories;
using PuzzleMania.ViewModels;

namespace PuzzleMania.Controllers
{
    public class TeamController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITeamRepository _teamRepository;

        public TeamController(IHttpContextAccessor httpContextAccessor, ITeamRepository teamRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _teamRepository = teamRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> JoinTeam()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            // Check if the user has already joined a team
            if (await _teamRepository.CheckIfUserHasTeam(currentUserId)) 
            {
                TempData["Message"] = "You have already joined a team.";
                return RedirectToAction("TeamStats");
            }

            // Retrieve the list of incomplete teams and than pass it to the view
            var incompleteTeams = await _teamRepository.GetIncompleteTeams();

            if (incompleteTeams.Count() > 0) //checks if there are any incomplete teams, if there are incomplete teams than user has to choose a team
            {
                return View("ChooseTeam", incompleteTeams);
            }

            // No incomplete teams & user has no team, show the form to create a new team
            return View("CreateTeam");
        }

        //this method shows the team stats(names) and gets called when there are incomplete teams and user has to choose a team
        //TODO - this method need change name to TeamList and it will be called when player has not yet joined a team && there are not full teams)
        [HttpGet]
        public async Task<IActionResult> ChooseTeam()
        {
            var chooseTeam = await _teamRepository.GetAll();
            return View(chooseTeam);
        }

      

        [HttpGet]
        public async Task<IActionResult> TeamStats(int teamId)
        {
            //TODO - why there is a null value
            var curTeam = await _teamRepository.GetByIdAsync(teamId);
           //if (curTeam == null) return View("Error");

            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            // Check if the user has already joined a team
            if (!await _teamRepository.CheckIfUserHasTeam(currentUserId))
            {
                TempData["Message"] = "You have not joined a team yet.";
                return RedirectToAction("JoinTeam");
            }

            // Retrieve the team the user has joined
            var team = await _teamRepository.GetTeamByUserId(currentUserId);

            // Retrieve the list of users in the team
            var teamMembers = await _teamRepository.GetTeamMembers(team.UserId);

            // Pass the team and team members to the view
            var teamStatsViewModel = new TeamStatsViewModel
            {
                TeamName= team.TeamName,
                TeamSize= team.TeamSize,
                TeamMembers = teamMembers,
                TotalPoints = team.TotalPoints
            };

            return View(teamStatsViewModel);
        }



        [HttpPost]
        public async Task<IActionResult> JoinTeam(string teamName)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();


            // Logic to handle the form submission for creating a new team
            /*if (!string.IsNullOrEmpty(teamName))
            {
                // Create the new team with the provided name and assaigning user to a team
                var newTeam = new Team
                {
                    TeamName = teamName,
                    UserId = currentUserId,
                    TeamSize = 1

                };
                

                _teamRepository.Add(newTeam);
                TempData["Message"] = "Team created successfully!";
                return RedirectToAction("TeamStats");
            }*/
           
                if (currentUserId != null)
                {
                    var teamToJoin = await _teamRepository.GetTeamByName(teamName);  // Retrieve the team the user wants to join based on their selection

                    if (teamToJoin != null && teamToJoin.TeamSize < 2)
                    {
                        await _teamRepository.AddUserToTeam(currentUserId, teamToJoin.TeamId);
                        await _teamRepository.IncreaseTeamSize(teamToJoin.TeamId);
                        _teamRepository.Save();

                        TempData["Message"] = "Joined the team successfully!";
                        return RedirectToAction("TeamStats");

                    }
                    else
                    {
                        TempData["Message"] = "Selected team does not exist!";
                        return View();
                    }
                }

                return View();
            


        }

        [HttpGet]
        public async Task<IActionResult> CreateTeam()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateTeam(string teamName)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
              
                // Create the new team with the provided name and assaigning user to a team
                var newTeam = new Team
                {
                    TeamName = teamName,
                    UserId = currentUserId,
                    TeamSize = 1
                };

                _teamRepository.Add(newTeam);
                TempData["Message"] = "Team created successfully!";
                return RedirectToAction("TeamStats");
            
        }
    }
}
