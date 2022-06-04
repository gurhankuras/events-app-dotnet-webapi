

namespace Auth.AsyncServices;


public interface IMessageBusClient {
    void PublishUser(UserPublishedDto userPublishedDto);
}