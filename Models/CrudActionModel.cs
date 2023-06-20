using BlogEngineNwareTechnologies.Models;

namespace WebApplication1.Models
{
    public class CrudActionModel
    {

        public enum ECrudAction
        {
            Create,
            Update, 
            Delete,
            Exist,
            Used,
            Empty,
            Incorrect,
            Error
        }

        public ECrudAction? CrudAction { get; set; }

    }
}
