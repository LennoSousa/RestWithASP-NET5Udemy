using RestWithASPNETUdemy.Data.VO;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RestWithASPNETUdemy.Data.VO
{
    [Table("person")]
    public class PersonVO
    {
        //para serializar os campos desejados, é necessário adicionar as seguintes notações em cada atributo.

        [JsonPropertyName("Code")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
        
        //para ignorar o campo e não serializar ele, basta adicionar a tag a baixo
        [JsonIgnore]
        public string Address { get; set; }
        [JsonPropertyName("sex")]
        public string Gender { get; set; }

        /*Essa serialização, serve tanto para retornar os dados ao cliente, quanto para
        persistir na base de dados, caso queiramos mudar os nomes, será necessário ajustar
        tbm as chamadas para a base.*/
    }
}
