using Infrastructure;
using System.Collections.Generic;

namespace Service
{
    public interface IPersonService : IService
    {
        PageResult<PersonDTO> GetPersonPage(PersonSearchParamDTO personSearchParamDTO, FyuUser user);
        bool AddPerson(PersonAddDTO personDTO, FyuUser user);
        bool IsPersonExist(string IdCard, FyuUser user,int mycompany = 0);
        bool DeletePerson(string personID);
        bool IsGroupPersonnelExist(string idCard);
        bool UpdatePerson(PersonDTO personDTO, FyuUser fyuUser);
        bool IsPersonExistByPersonID(string personID);
        bool IsPersonExist(string IdCard, string personID);
        PersonStatisticsDTO GetPersonStatistics(FyuUser user);
        byte[] GetPersonInfoTemplate();
        List<PersonDTO> GetPersonByIDCard(List<string> IDCardList, FyuUser user);
        (bool, string,int) ImportPersonInfo(List<PersonImportDTO> personDTOs, FyuUser user);
        List<CompanyForTreeDTO> GetOrganization(FyuUser fyuUser);
        bool GetPersonWeChatBind();
        List<ExecuteResult2DTO> GetOrganizationNew(FyuUser user);


        /// <summary>
        /// 姓名或身份证获取人员列表（添加补卡人员）
        /// </summary>
        /// <param name="nameOrIDCard"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        List<SupplementCardDTO> GetSupplementCard(string nameOrIDCard, FyuUser user);
    }
}
