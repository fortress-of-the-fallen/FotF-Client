#if UNITY_2019_1_OR_NEWER
using System.Threading.Tasks;

namespace Game.Application.Interface
{
	public interface IFeature<TInput, TOutput>
	{
		Task<TOutput> Execute(TInput input);
	}
}
#endif

