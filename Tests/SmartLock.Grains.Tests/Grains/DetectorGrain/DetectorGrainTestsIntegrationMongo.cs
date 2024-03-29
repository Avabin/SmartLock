﻿using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.Grains.DetectorGrain;

[Category("Integration"), Category("Mongo"), TestFixture,Parallelizable(ParallelScope.Children)]
public class DetectorGrainTestsIntegrationMongo : DetectorGrainTestsBase<MongoTestSiloBuilderConfigurator, MongoTestsClientConfigurator>
{
    
}