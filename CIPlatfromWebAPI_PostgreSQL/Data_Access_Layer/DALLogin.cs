using Data_Access_Layer.Repository;
using Data_Access_Layer.Repository.Entities;
using System.Data;

namespace Data_Access_Layer
{
    public class DALLogin
    {
        private readonly AppDbContext _cIDbContext;
        public DALLogin(AppDbContext cIDbContext)
        {
            _cIDbContext = cIDbContext;
        }

        public string Register(User user)
        {
            string result = string.Empty;
            try
            {
                bool emailExists = _cIDbContext.User.Any(u => u.EmailAddress == user.EmailAddress && !u.IsDeleted);
                if (!emailExists)
                {
                    string maxEmployeeIdStr = _cIDbContext.UserDetail.Max(ud => ud.EmployeeId);
                    int maxEmployeeId = 0;

                    if(!string.IsNullOrEmpty(maxEmployeeIdStr))
                    {
                        if(int.TryParse(maxEmployeeIdStr, out int parsedEmployeeId))
                        {
                            maxEmployeeId = parsedEmployeeId;
                        }
                        else
                        {
                            throw new Exception("Error while converting string to int.");
                        }
                    }

                    int newEmployeeId = maxEmployeeId + 1;

                    var newUser = new User
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        EmailAddress = user.EmailAddress,
                        Password = user.Password,
                        UserType = user.UserType,
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    _cIDbContext.User.Add(newUser);
                    _cIDbContext.SaveChanges();


                    var newUserDetail = new UserDetail
                    {
                        UserId = newUser.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        EmailAddress = user.EmailAddress,
                        UserType = user.UserType,
                        Name = user.FirstName,
                        Surname = user.LastName,
                        EmployeeId = newEmployeeId.ToString(),
                        Department = "IT",
                        Status = true
                    };

                    _cIDbContext.UserDetail.Add(newUserDetail);
                    _cIDbContext.SaveChanges();

                    result = "User Register Successfully";
                }
                else
                {
                    throw new Exception("Email already exist.");
                }
            }

            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public User LoginUser(User user)
        {
            User userObj = new User();

            try
            {
                var query = from u in _cIDbContext.User
                            where u.EmailAddress == user.EmailAddress && u.IsDeleted == false
                            select new
                            {
                                u.Id,
                                u.FirstName,
                                u.LastName,
                                u.PhoneNumber,
                                u.EmailAddress,
                                u.UserType,
                                u.Password,
                                UserImage = u.UserImage
                            };

                var userData = query.FirstOrDefault();

                if (userData != null)
                {
                    if (userData.Password == user.Password)
                    {
                        userObj.Id = userData.Id;
                        userObj.FirstName = userData.FirstName;
                        userObj.LastName = userData.LastName;
                        userObj.PhoneNumber = userData.PhoneNumber;
                        userObj.EmailAddress = userData.EmailAddress;
                        userObj.UserType = userData.UserType;
                        userObj.UserImage = userData.UserImage;
                        userObj.Message = "Login Successfully";
                    }
                    else
                    {
                        userObj.Message = "Incorrect Password.";
                    }
                }
                else
                {
                    userObj.Message = "Email Address Not Found.";
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return userObj;
        }

        public User GetUserById(int id)
        {
            User userObj = new User();
            try
            {
                var query = from u in _cIDbContext.User
                            where u.Id == id && u.IsDeleted == false
                            select u;

                userObj = query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return userObj;
        }

        public User UpdateUser(User user)
        {
            User userObj = new User();
            using (var transaction = _cIDbContext.Database.BeginTransaction())
            {
                try
                {
                    var userData = _cIDbContext.User.FirstOrDefault(u => u.Id == user.Id && !u.IsDeleted);
                    var userDetails = _cIDbContext.UserDetail.FirstOrDefault(u => u.UserId == user.Id && !u.IsDeleted);

                    if (userData != null && userDetails != null)
                    {
                        // Update User
                        userData.FirstName = user.FirstName;
                        userData.LastName = user.LastName;
                        userData.PhoneNumber = user.PhoneNumber;
                        userData.EmailAddress = user.EmailAddress;
                        userData.Password = user.Password;

                        // Update UserDetail
                        userDetails.FirstName = user.FirstName;
                        userDetails.LastName = user.LastName;
                        userDetails.PhoneNumber = user.PhoneNumber;
                        userDetails.EmailAddress = user.EmailAddress;
                        userDetails.Name = user.FirstName;
                        userDetails.Surname = user.LastName;

                        _cIDbContext.SaveChanges();
                        transaction.Commit();
                        userObj.Message = "User Updated Successfully";
                    }
                    else
                    {
                        userObj.Message = "User Not Found.";
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return userObj;
        }

        public string LoginUserProfileUpdate(UserDetail userDetail)
        {
            string result = "";
            using (var transaction = _cIDbContext.Database.BeginTransaction())
            {
                try
                {
                    var existingUserDetail = _cIDbContext.UserDetail.FirstOrDefault(u => u.UserId == userDetail.Id && !u.IsDeleted);
                    var existingUserData = _cIDbContext.User.FirstOrDefault(u => u.Id == userDetail.Id && !u.IsDeleted);

                    if (existingUserDetail != null && existingUserData != null)
                    {
                        existingUserDetail.Name = userDetail.Name;
                        existingUserDetail.Surname = userDetail.Surname;
                        existingUserDetail.EmployeeId = userDetail.EmployeeId;
                        existingUserDetail.Manager = userDetail.Manager;
                        existingUserDetail.Title = userDetail.Title;
                        existingUserDetail.Department = userDetail.Department;
                        existingUserDetail.MyProfile = userDetail.MyProfile;
                        existingUserDetail.WhyIVolunteer = userDetail.WhyIVolunteer;
                        existingUserDetail.CountryId = userDetail.CountryId;
                        existingUserDetail.CityId = userDetail.CityId;
                        existingUserDetail.Avilability = userDetail.Avilability;
                        existingUserDetail.LinkdInUrl = userDetail.LinkdInUrl;
                        existingUserDetail.MySkills = userDetail.MySkills;
                        existingUserDetail.UserImage = userDetail.UserImage;
                        existingUserDetail.Status = userDetail.Status;
                        existingUserDetail.ModifiedDate = DateTime.UtcNow;

                        existingUserData.FirstName = userDetail.Name;
                        existingUserData.LastName = userDetail.Surname;

                        _cIDbContext.SaveChanges();
                        transaction.Commit();
                        result = "Account Update Successfully.";
                    }
                    else
                    {
                        result = "Account Detail is not found.";
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return result;
        }
    }
}
