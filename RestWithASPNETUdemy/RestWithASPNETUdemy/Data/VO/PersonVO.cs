using RestWithASPNETUdemy.Data.VO;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RestWithASPNETUdemy.Data.VO
{
    [Table("person")]
    public class PersonVO
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
    }
}
