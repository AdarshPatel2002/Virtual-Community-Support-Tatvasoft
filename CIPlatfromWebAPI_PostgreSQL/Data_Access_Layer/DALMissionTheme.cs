using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Repository.Entities;
using Data_Access_Layer.Repository;

namespace Data_Access_Layer
{
    public class DALMissionTheme
    {
        private readonly AppDbContext _cIDbContext;

        public DALMissionTheme(AppDbContext cIDbContext)
        {
            _cIDbContext = cIDbContext;
        }

        public async Task<List<MissionTheme>> GetMissionThemeListAsync()
        {
            return await _cIDbContext.MissionTheme.Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task<MissionTheme> GetMissionThemeByIdAsync(int id)
        {
            return await _cIDbContext.MissionTheme.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<string> AddMissionThemeAsync(MissionTheme missionTheme)
        {
            try
            {
                _cIDbContext.MissionTheme.Add(missionTheme);
                await _cIDbContext.SaveChangesAsync();
                return "Save Theme Successfully.";
            }
            catch (Exception ex)
            {
                throw new Exception("Error in adding Theme.", ex);
            }
        }

        public async Task<string> UpdateMissionThemeAsync(MissionTheme missionTheme)
        {
            try
            {
                var existingTheme = await _cIDbContext.MissionTheme.Where(x => !x.IsDeleted && x.Id == missionTheme.Id).FirstOrDefaultAsync();
                if (existingTheme != null)
                {
                    existingTheme.ThemeName = missionTheme.ThemeName;
                    existingTheme.Status = missionTheme.Status;
                    await _cIDbContext.SaveChangesAsync();
                    return "Update Theme Successfully.";
                }
                else
                {
                    throw new Exception("Mission Theme is not found.");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error in updating Theme.", ex);
            }
        }

        public async Task<string> DeleteMissionThemeAsync(int id)
        {
            try
            {
                var existingTheme = await _cIDbContext.MissionTheme.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
                if (existingTheme != null)
                {
                    existingTheme.IsDeleted = true;
                    await _cIDbContext.SaveChangesAsync();
                    return "Delete Theme Successfully.";
                }
                else
                {
                    throw new Exception("Mission Theme is not found.");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error in deleting Theme.", ex);
            }
        }
    }
}
