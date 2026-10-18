# Production Note

The included server is a complete architectural foundation and runnable API skeleton, but it intentionally does not pretend that an in-memory sample is a production economy. Before live launch, replace the sample in-memory operations with your real Master Authority + PostgreSQL atomic transaction calls.

The production settlement must happen in one authoritative database transaction where appropriate. Never ship the sample service as the final live economy without connecting those domain services.
