using GChan.Models.Trackers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;

namespace GChan.Data
{
    [PrimaryKey(nameof(Site), nameof(Code))]
    public class BoardData
    {
        public Site Site { get; set; }

        /// <summary>
        /// The code of the board with no slashes (e.g. "wsg").
        /// </summary>
        public string Code { get; set; }

        public long GreatestThreadId { get; set; }

        /// <summary>
        /// Parameterless constructor for Entity Framework.
        /// </summary>
        public BoardData() { }

        public BoardData(Board board)
        {
            this.Site = board.Site;
            this.Code = board.BoardCode.Trim('/');
            this.GreatestThreadId = board.GreatestThreadId;
        }

        public Expression<Func<BoardData, bool>> EfEquals(BoardData other)
        {
            return b => b.Site == other.Site && b.Code == other.Code;
        }
    }
}
