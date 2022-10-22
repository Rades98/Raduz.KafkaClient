using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Raduz.KafkaClient.Contracts.Requests
{
	public interface IKafkaClientRequest : IRequest<bool>
	{
	}
}
