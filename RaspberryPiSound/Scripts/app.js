var app = angular.module('raspberryPiSoundApp', []);


function refresh($http, $scope)
{
    $http.get('/api/Volume').success(function (result) {
        $scope.Volume = result;
    });

    $http.get('/api/MusicPlayer').success(function (result) {
        $scope.currentTrack = result;
    });
}

function switchDirectory($http, $scope, dir)
{
    $scope.isBusy = true;
    $http.get('/api/Directory?Path=' + dir).success(function (result) {
        $scope.Directories = result.Directories;
        $scope.MusicFiles = result.Files;
        $scope.currentDir = result.CurrentPath;
        $scope.isBusy = false;
        $scope.init = true;
    });
    refresh($http, $scope);
}

app.controller('musicController', function ($scope, $http, $timeout) {
    $scope.isBusy = true;
    $scope.init = false;
    $scope.currentDir = "";
    $scope.currentTrack = "";

    $scope.SwitchDirectory = function (dir) {
        var current = $scope.currentDir;
        if (current.length > 0 && current.substr(current.length - 1) != "/")
            current += "/";

        current += dir.Name + "/";
        switchDirectory($http, $scope, current);
    };

    $scope.SwitchDirectoryBack = function () {
        var current = $scope.currentDir;
        var idx = current.lastIndexOf("/");
        current = current.substring(0, idx);
        idx = current.lastIndexOf("/");
        current = current.substring(0, idx);
        switchDirectory($http, $scope, current);
    };

    $scope.PlayAll = function (files) {
        $http.post('/api/Playlist', files);
        refresh($http, $scope);
    };

    $scope.PlaySingle = function (file) {
        $http.post('/api/Playlist', [file]);
        refresh($http, $scope);
    };


    $scope.PlayNext = function (files) {
        $http.post('/api/MusicPlayer', {
            Action: 'Next'
        });
        refresh($http, $scope);
    };
    
    $scope.PauseMusic = function (file) {
        $http.post('/api/MusicPlayer', {
            Action: 'Pause'
        });
        refresh($http, $scope);
    };

    $scope.StopMusic = function (file) {
        $http.post('/api/MusicPlayer', {
            Action: 'Stop'
        });
        refresh($http, $scope);
    };


    $scope.VolumeUp = function () {
        $http.post('/api/volume?up=true').success(function (data) {
            $scope.Volume = data;
        });
    };

    $scope.VolumeDown = function () {
        $http.post('/api/volume?up=false').success(function (data) {
            $scope.Volume = data;
        });
    };

    switchDirectory($http, $scope, $scope.currentDir);
    refresh($http, $scope);

    var countUp = function() {
        refresh($http, $scope);
        $timeout(countUp, 5000);
    };

    $timeout(countUp, 5000);
});