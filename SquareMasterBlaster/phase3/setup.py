import ez_setup 
ez_setup.use_setuptools()

from setuptools import setup, find_packages
setup(
    name = "Square Master Blasters",
    version = "1.0",
    packages = find_packages(),
    scripts = ['SquareMasterBlaster.py'],

    # Project uses reStructuredText, so ensure that the docutils get
    # installed or upgraded on the target machine
    install_requires = ['docutils>=0.3', 'pygame'],

    package_data = {
        # If any package contains *.txt or *.rst files, include them:
        '': ['*.txt', '*.rst'],
        # And include any *.msg files found in the 'hello' package, too:
        'hello': ['*.msg'],
    },

    # metadata for upload to PyPI
    author = "Will, Bridget, Brady, Moad",
    author_email = "bradyschnell@gmail.com",
    description = "CHANGE THIS LATER",
    license = "PSF",
    keywords = "",
    url = "https://github.com/willynch/SENG330",   # project home page, if any

    # could also include long_description, download_url, classifiers, etc.
)
