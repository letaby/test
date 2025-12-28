namespace Softing.Dts;

public interface DtsJavaProgCodeInfo
{
	byte[] JarFile { get; }

	uint NoOfLibraryFiles { get; }

	string PackageEntry { get; }

	string JarFileName { get; }

	string JobFileRevision { get; }

	byte[] GetLibraryFile(uint index);

	bool UseClassLoaderInternalJar();

	string GetLibraryFileName(uint index);
}
