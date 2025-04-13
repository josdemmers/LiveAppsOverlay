﻿using CommunityToolkit.Mvvm.Messaging;
using LiveAppsOverlay.Entities;
using LiveAppsOverlay.Interfaces;
using LiveAppsOverlay.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiveAppsOverlay.Services
{
    public class ReleaseManager : IReleaseManager
    {
        private readonly ILogger _logger;
        private readonly IHttpClientHandler _httpClientHandler;
        private readonly ISettingsManager _settingsManager;

        private List<Release> _releases = new List<Release>();
        private bool _updateAvailable = false;

        // Start of Constructors region

        #region Constructors

        public ReleaseManager(ILogger<ReleaseManager> logger, IHttpClientHandler httpClientHandler, ISettingsManager settingsManager)
        {
            // Init services
            _httpClientHandler = httpClientHandler;
            _logger = logger;
            _settingsManager = settingsManager;

            // Update release info
            Task.Factory.StartNew(() =>
            {
                UpdateAvailableReleases();
            });
        }

        #endregion

        // Start of Events region

        #region Events

        #endregion

        // Start of Properties region

        #region Properties

        public List<Release> Releases { get => _releases; set => _releases = value; }
        public string Repository { get; } = "https://api.github.com/repos/josdemmers/liveappsoverlay/releases";
        public bool UpdateAvailable { get => _updateAvailable; set => _updateAvailable = value; }

        #endregion

        // Start of Event handlers region

        #region Event handlers

        #endregion

        // Start of Methods region

        #region Methods

        private async void UpdateAvailableReleases()
        {
            try
            {
                if (_settingsManager.Settings.IsCheckForUpdatesEnabled)
                {
                    _logger.LogInformation($"Updating release info from: {Repository}");

                    string json = await _httpClientHandler.GetRequest(Repository);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        Releases.Clear();
                        Releases = JsonSerializer.Deserialize<List<Release>>(json) ?? new List<Release>();
                    }
                    else
                    {
                        _logger.LogWarning($"Invalid response. uri: {Repository}");
                    }
                    WeakReferenceMessenger.Default.Send(new ReleaseInfoUpdatedMessage());
                }
                else
                {
                    _logger.LogInformation($"Check for updates disabled by user.");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, MethodBase.GetCurrentMethod()?.Name);
            }
        }

        #endregion
    }
}
