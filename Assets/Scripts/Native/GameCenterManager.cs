using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;

public class GameCenterManager : MonoBehaviour
{

    public static GameCenterManager instance { get; private set; }

    public bool m_IsAvailable = false;
    public bool m_IsAuthenticated = false;

    private Dictionary<string, string> m_ActiveAchievements =
        new Dictionary<string, string>();

    protected void OnEnable()
    {
        if (GameCenterManager.instance == null)
        {
            GameCenterManager.instance = this;
        }
        else if (GameCenterManager.instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Awake()
    {
        m_IsAvailable = NPBinding.GameServices.IsAvailable();
        this.LoadAchievements();
    }

    void Start()
    {
        this.SignIn();
    }

    public void SignIn()
    {
        if (!m_IsAvailable) return;

        m_IsAuthenticated = NPBinding.GameServices.LocalUser.IsAuthenticated;
        if (!m_IsAuthenticated)
        {
            //Authenticate Local User
            NPBinding.GameServices.LocalUser.Authenticate(
                (bool _success, string _error) =>
                {

                    if (_success)
                    {
                        m_IsAuthenticated = true;
                        Debug.Log("Sign-In Successfully");
                        Debug.Log("Local User Details: " + NPBinding.GameServices.LocalUser.ToString());
                    }
                    else
                    {
                        Debug.Log("Sign-In Failed with error " + _error);
                    }
                });
        }
    }

    public bool TrySignOut()
    {
        if (!m_IsAvailable || !m_IsAuthenticated) return false;

        bool result = false;
        NPBinding.GameServices.LocalUser.SignOut(
            (bool _success, string _error) =>
            {
                if (_success)
                {
                    m_IsAuthenticated = false;
                    Debug.Log("Local user is signed out successfully!");
                }
                else
                {
                    Debug.Log("Request to signout local user failed.");
                    Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
                }
            });

        return result;
    }

    public bool TryGetFriends(out List<User> friendsList)
    {
        friendsList = new List<User>();

        if (!m_IsAvailable || !m_IsAuthenticated) return false;

        List<User> temp = new List<User>();
        bool result = false;
        NPBinding.GameServices.LocalUser.LoadFriends(
            (User[] _friendsList, string _error) =>
            {
                if (_friendsList != null)
                {
                    Debug.Log("Succesfully loaded user friends.");
                    foreach (User _eachFriend in _friendsList)
                    {
                        Debug.Log(string.Format("Name: {0}, Id: {1}",
                            _eachFriend.Name, _eachFriend.Identifier));
                    }

                    result = true;
                    temp = new List<User>(_friendsList);
                }
                else
                {
                    result = false;
                    Debug.Log("Failed to load user friends with error " + _error);
                }

            });

        friendsList = new List<User>(temp);
        return result;
    }
    public bool TryGetUsersByIDs(List<string> IDsList, out List<User> usersList)
    {
        usersList = new List<User>();

        if (!m_IsAvailable || !m_IsAuthenticated) return false;

        List<User> temp = new List<User>();
        bool result = false;
        NPBinding.GameServices.LoadUsers(
            IDsList.ToArray(), (User[] _users, string _error) =>
            {
                if (_users != null)
                {
                    Debug.Log("Succesfully loaded users info.");
                    foreach (User _eachUser in _users)
                    {
                        Debug.Log(string.Format("Name: {0}, Id: {1}", _eachUser.Name, _eachUser.Identifier));
                    }
                    result = true;
                    temp = new List<User>(_users);
                }
                else
                {
                    result = false;
                    Debug.Log("Failed to load users info with  error = " + _error);
                }

            });

        usersList = new List<User>(temp);
        return result;
    }

    #region Achievements
    private void LoadAchievements()
    {
        if (!m_IsAvailable) return;

        NPBinding.GameServices.LoadAchievements((Achievement[] _achievements, string _error) =>
        {

            if (_achievements == null)
            {
                Debug.Log("Couldn't load achievement list with error = " + _error);
                return;
            }

            int _achievementCount = _achievements.Length;
            Debug.Log(string.Format("Successfully loaded achievement list. Count={0}.", _achievementCount));

            m_ActiveAchievements.Clear();
            for (int _iter = 0; _iter < _achievementCount; _iter++)
            {
                m_ActiveAchievements.Add(_achievements[_iter].GlobalIdentifier,
                    _achievements[_iter].Identifier);
                Debug.Log(string.Format("[Index {0}]: {1}", _iter, _achievements[_iter]));
            }
        });
    }

    public void ReportAchievement(string globalID)
    {
        if (!m_IsAvailable) return;
        if (!m_ActiveAchievements.ContainsKey(globalID)) return;

        NPBinding.GameServices.ReportProgressWithGlobalID(globalID, 100d,
            (bool _status, string _error) =>
            {

                if (_status)
                {
                    Debug.Log(string.Format("Request to report progress of achievement with GID= {0} finished successfully.", globalID));
                    Debug.Log(string.Format("Percentage completed= {0}.", 100d));
                }
                else
                {
                    Debug.Log(string.Format("Request to report progress of achievement with GID= {0} failed.", globalID));
                    Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
                }
            });
    }

    public void ShowAchievementsUI()
    {
        if (!m_IsAvailable) return;

        NPBinding.GameServices.ShowAchievementsUI((string _error) =>
        {
            Debug.Log("Achievements view dismissed.");
            if (!string.IsNullOrEmpty(_error))
                Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
        });
    }
    #endregion

    #region Leaderboards
        
    #endregion

}